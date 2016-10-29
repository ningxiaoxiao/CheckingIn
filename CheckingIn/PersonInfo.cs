using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using LitJson;

namespace CheckingIn
{
    public class PersonInfo
    {

        /// <summary>
        /// 异常天数
        /// </summary>
        public int WarnDayCount
        {
            get
            {
                if (_warnDayCount != -1) return _warnDayCount;
                _warnDayCount = 0;
                foreach (var c in Checks)
                {
                    if (c.HaveWarn)
                        _warnDayCount++;

                }
                return _warnDayCount;
            }
        }

        //总属性
        public string Name;

        public string Mail;

        /// <summary>
        /// 可用假期
        /// </summary>
        public TimeSpan Holidays = TimeSpan.Zero;

        private TimeSpan _overWorkTime = new TimeSpan(-1);
        /// <summary>
        /// 加班时间
        /// </summary>
        public TimeSpan OverWorkTime
        {
            get
            {
                if (_overWorkTime >= TimeSpan.Zero) return _overWorkTime;


                var dv = new DataView(DB.OaDt) { RowFilter = "name='" + Name + "' and reason = '加班'" };
                //所有数据应该已经合法
                _overWorkTime = TimeSpan.Zero;

                foreach (DataRowView item in dv)
                {
                    var s = (DateTime)item["start"];
                    var e = (DateTime)item["end"];


                    _overWorkTime += e - s;
                }

                return _overWorkTime;
            }
        }

        //当月属性

        private TimeSpan _travel = new TimeSpan(-1);
        /// <summary>
        /// 出差天数
        /// </summary>
        public TimeSpan Travel
        {
            get
            {
                if (_travel >= TimeSpan.Zero) return _travel;

                //找出所有出差
                var dv = new DataView(DB.OaDt) { RowFilter = "name='" + Name + "' and reason = '出差'" };
                //所有数据应该已经合法
                _travel = TimeSpan.Zero;
                foreach (DataRowView item in dv)
                {
                    var s = (DateTime)item["start"];
                    var e = (DateTime)item["end"];


                    _travel += e.Date.AddDays(1) - s.Date;
                }
                return _travel;
            }
        }
        /// <summary>
        /// 迟到早退时间
        /// </summary>
        public TimeSpan DelayTime = TimeSpan.Zero;


        public int WorkDayCount => WorkDay.WorkCount - WarnDayCount;

        /// <summary>
        /// 工作时间
        /// </summary>
        public TimeSpan WorkTime = TimeSpan.Zero;


        /// <summary>
        /// 工作时间班次
        /// </summary>
        public WorkTimeClassInfo WorkTimeClass;

        public List<CheckInfo> Checks = new List<CheckInfo>();

        public PersonInfo(string n)
        {
            Name = n;
            //从db中得到相关信息
            var dv = new DataView(DB.PersonInfos) { RowFilter = $"name ='{Name}'" };
            if (dv.Count == 0)
            {
                Mail = null;
                WorkTimeClass = WorkTimeClassInfo.Default;
                Log.Err(n + "-无法得到个人信息");
            }
            else
            {

                Mail = dv[0]["mail"].ToString();


                var wtc = dv[0]["worktimeclass"].ToString();

                WorkTimeClass = new WorkTimeClassInfo(wtc);

                if (Mail == "")
                    Log.Err(n + "-mail err");
                if (wtc == "")
                    Log.Err(n + "-worktimeclass err");

            }

        }

        public void AddCheck(CheckInfo c)
        {
            Checks.Add(c);
            WorkTime += c.WorkTime;
        }


        //处理这个人的数据
        public void GetData()
        {
            if (_geted) return;

            foreach (var i in WorkDay.AllDays)
            {
                //今日日期
                var date = i.Key;
                var c = new CheckInfo(this, date, null, null);


                //得到这个人今天所有的打卡时间
                var timeview = new DataView(DB.OriginalDt)
                {
                    RowFilter = $"name = '{Name}' AND date = '{date}'",
                    Sort = "time asc" //从小到大
                };

                c.Sourcerec = timeview;
                foreach (DataRowView t in timeview)
                {
                    var r = t["info"].ToString();
                    if (!c.Info.Contains(r))
                    {
                        c.Info += r + " ";
                    }

                }
                switch (timeview.Count)
                {
                    case 0:
                        c.InTime = null;
                        c.OutTime = null;
                        break;
                    case 1:
                        c.InTime = null;
                        c.OutTime = null;

                        //判断是上班,还是下班

                        var t = (TimeSpan)timeview[0].Row["time"];

                        if (t > WorkTimeClass.InTime + new TimeSpan(0, 4, 0, 0))
                            c.OutTime = t;
                        else
                            c.InTime = t;



                        if (WorkTimeClass.IsWorkTimeClass)
                            Log.Info(Name + "-单次打卡");
                        break;

                    default:
                        c.InTime = (TimeSpan)timeview[0].Row["time"];
                        c.OutTime = (TimeSpan)timeview[timeview.Count - 1].Row["time"];
                        break;
                }
                AddCheck(c);
            }
            _geted = true;

        }


        public JsonData GetJson()
        {
            var j = new JsonData();

            var js = new JsonWriter();
            var csjs = new JsonWriter();
            try
            {

                js.WriteArrayStart();
                csjs.WriteArrayStart();

                //sort
                Checks.Sort();

                foreach (var c in Checks)
                {
                    foreach (var w in c.Warns)
                    {
                        js.WriteObjectStart();
                        js.WritePropertyName("date");
                        js.Write(w.Date.ToString());
                        js.WritePropertyName("info");
                        js.Write(w.Info);
                        js.WriteObjectEnd();
                    }

                    csjs.WriteObjectStart();

                    csjs.WritePropertyName("date");
                    csjs.Write(c.Date.ToString());

                    csjs.WritePropertyName("intime");
                    csjs.Write(c.InTime?.ToString() ?? "");

                    csjs.WritePropertyName("outtime");
                    csjs.Write(c.OutTime?.ToString() ?? "");

                    csjs.WritePropertyName("info");
                    csjs.Write(c.Info);

                    csjs.WriteObjectEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            csjs.WriteArrayEnd();
            js.WriteArrayEnd();

            var profile = new JsonData();

            profile["姓名"] = Name;
            profile["剩余假期"] = Holidays.TotalHours.ToString("0.#") + "小时";
            profile["工作班次"] = WorkTimeClass.ToString();


            if (WorkTimeClass.IsWorkTimeClass)
                profile["工作时间"] = WorkTime.TotalHours.ToString("0.#") + "/" + WorkDay.WorkCount * 8 + "小时";
            else
            {
                profile["工作天数"] = WorkDayCount.ToString("0") + "/" + WorkDay.WorkCount + "天";
                profile["异常天数"] = WarnDayCount.ToString("0 '天'");
                profile["迟到早退"] = DelayTime.TotalMinutes.ToString("0.# '分钟'");
            }
               
            profile["使用调休假期"] = "未接入";
            profile["使用扣薪假期"] = "未接入";
            profile["加班"] = OverWorkTime.TotalHours.ToString("0.# '小时'");
            profile["出差"] = Travel.TotalDays.ToString("0.# '天'");

         


            j["profile"] = profile;



            j["warns"] = js.ToString();
            j["data"] = csjs.ToString();

            return j;

        }


        public string GetText()
        {
            var j = GetJson();

            var p = (IDictionary)j[0];
            var r = p.Keys.Cast<object>().Aggregate("综合信息\r\n", (current, k) => current + (k + " - " + p[k] + "\r\n"));

            r += "\r\n异常信息\r\n";

            var w = JsonMapper.ToObject(j[1].ToString());
            r = w.Cast<JsonData>().Aggregate(r, (current, d) => current + (d[0] + " - " + d[1] + "\r\n"));

            r += "\r\n原始信息\r\n";
            var ds = JsonMapper.ToObject(j[2].ToString());

            r = ds.Cast<JsonData>().Aggregate(r, (current, djd) => current + (djd[0] + " - " + (djd[1].ToString() == "" ? "notfind" : djd[1]) + " - " + (djd[2].ToString() == "" ? "notfind" : djd[2]) + djd[3] + "\r\n"));

            return r;
        }
        public CheckInfo GetCheck(WorkDay d)
        {

            foreach (var c in Checks)
            {
                if (c.Date == (DateTime)d)
                    return c;
            }
            return null;
        }
        public override string ToString()
        {
            return Name;
        }

        private int _warnDayCount = -1;
        private bool _geted;
    }
    /// <summary>
    /// 工作班次信息
    /// </summary>
    public class WorkTimeClassInfo
    {
        public string ClassName;
        public CheckTime InTime;
        public CheckTime OutTime;
        public bool IsWorkTimeClass => ClassName == "综合班次";
        public WorkTimeClassInfo(string n)
        {
            ClassName = n;
            switch (n)
            {
                case "早班":
                    InTime = new TimeSpan(0, 9, 0, 0);
                    OutTime = new TimeSpan(0, 18, 0, 0);
                    break;
                case "中班":
                    InTime = new TimeSpan(0, 9, 30, 0);
                    OutTime = new TimeSpan(0, 18, 30, 0);
                    break;
                case "晚班":
                    InTime = new TimeSpan(0, 11, 30, 0);
                    OutTime = new TimeSpan(0, 20, 30, 0);
                    break;
                case "特别班次":
                    InTime = new TimeSpan(0, 12, 0, 0);
                    OutTime = new TimeSpan(0, 21, 0, 0);
                    break;
                case "综合班次":
                    InTime = TimeSpan.Zero;
                    OutTime = TimeSpan.Zero;
                    break;
                default:
                    InTime = new TimeSpan(0, 9, 0, 0);
                    OutTime = new TimeSpan(0, 18, 0, 0);
                    break;
            }
        }

        public static WorkTimeClassInfo Default = new WorkTimeClassInfo("早班");

        public override string ToString()
        {
            return ClassName;
        }
    }

    public struct WarnInfo
    {
        public WorkDay Date;
        public string Info;
        public WarnInfoType Type;
        public WarnInfo(WorkDay d, string i, WarnInfoType t)
        {
            Date = d;
            Info = i;
            Type = t;

        }
        public WarnInfo(WorkDay d, string i)
        {
            Date = d;
            Info = i;
            Type = WarnInfoType.Warn;
        }
    }

    /// <summary>
    /// 打卡时间
    /// </summary>
    public class CheckTime
    {
        public TimeSpan Time { get; set; }
        public CheckTime(TimeSpan t)
        {
            Time = t;
        }

        public CheckTime(string t)
        {
            Time = TimeSpan.Parse(t);
        }

        public static bool operator >(CheckTime a, CheckTime b)
        {
            try
            {
                return a.Time > b.Time;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool operator <(CheckTime a, CheckTime b)
        {
            try
            {
                return a.Time < b.Time;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static CheckTime operator +(CheckTime a, CheckTime b)
        {
            try
            {
                return new CheckTime(a.Time + b.Time);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static CheckTime operator -(CheckTime a, CheckTime b)
        {
            try
            {
                return new CheckTime(a.Time - b.Time);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static explicit operator TimeSpan(CheckTime a)
        {
            return a.Time;
        }
        public static implicit operator CheckTime(TimeSpan a)
        {
            return new CheckTime(a);
        }

        public override string ToString()
        {
            return Time.ToString();
        }
    }

    public enum WarnInfoType
    {
        Info,
        Warn
    }

    /// <summary>
    /// 打卡记录
    /// </summary>
    public class CheckInfo : IComparable<CheckInfo>
    {
        public PersonInfo Person;

        public WorkDay Date;



        public CheckTime InTime;
        public CheckTime OutTime;

        public TimeSpan WorkTime
        {
            get
            {
                try
                {
                    return (TimeSpan)(OutTime - InTime);
                }
                catch (Exception)
                {

                    return TimeSpan.Zero;
                }
            }
        }

        private List<WarnInfo> _warns;
        public List<WarnInfo> Warns
        {
            get
            {
                if (_warns != null) return _warns;

                _warns = new List<WarnInfo>();
                //不是工作日直接返回
                if (!Date.IsWorkDay) return _warns;
                //使用弹性工作制                                                      
                if (Person.WorkTimeClass.IsWorkTimeClass) return _warns;

                //都是空且,还是上班
                if (InTime == null && OutTime == null && Date.IsWorkDay)
                {
                    _warns.Add(new WarnInfo(Date, "旷工"));
                    return _warns;
                }


                if (InTime == null)
                {
                    _warns.Add(new WarnInfo(Date, "上班未打卡"));
                    return _warns;
                }
                if (OutTime == null)
                {
                    _warns.Add(new WarnInfo(Date, "下班未打卡"));
                    return _warns;
                }



                var k = TimeSpan.Zero;

                if (InTime > Person.WorkTimeClass.InTime)
                {

                    k = (TimeSpan)(InTime - Person.WorkTimeClass.InTime);

                    if (k > new TimeSpan(0, 0, 30, 0))//每天30分钟机动时间
                    {

                        var dt = k - new TimeSpan(0, 0, 30, 0);

                        Person.DelayTime += dt;
                        _warns.Add(new WarnInfo(Date, $"迟到{dt.TotalMinutes.ToString("0.#")}分钟"));

                        k = new TimeSpan(0, 0, 30, 0);


                    }


                }

                var shoudout = Person.WorkTimeClass.OutTime + k;

                if (OutTime < shoudout)
                {
                    k = (TimeSpan)(shoudout - OutTime);
                    Person.DelayTime += k;

                    _warns.Add(new WarnInfo(Date, $"早退{k.TotalMinutes.ToString("0.#")}分钟"));
                }

                return _warns;


            }
        }

        public bool HaveWarn => Warns.Count > 0;

        public string Info;
        public DataView Sourcerec;

        public CheckInfo(PersonInfo p, WorkDay d, CheckTime it, CheckTime ot, string info = "")
        {
            Person = p;
            Date = d;

            InTime = it;
            OutTime = ot;
            Info = info;
        }
        public CheckInfo(PersonInfo p, WorkDay d)
            : this(p, d, null, null)
        {

        }



        public int CompareTo(CheckInfo other)
        {
            return Date.CompareTo(other.Date);
        }
    }

    public class WorkDay : IComparable<WorkDay>
    {
        /// <summary>
        /// 有人出勤的日期
        /// </summary>
        public static Dictionary<DateTime, bool> AllDays = new Dictionary<DateTime, bool>();

        private static int _workcount = -1;
        public static int WorkCount
        {
            get
            {
                if (_workcount != -1) return _workcount;

                _workcount = 0;
                foreach (var i in AllDays)
                {
                    if (i.Value)
                        _workcount++;
                }
                return _workcount;
            }
        }

        public bool IsWorkDay => AllDays[_date];
        private readonly DateTime _date;

        public WorkDay(DateTime a)
        {
            _date = a.Date;
        }
        protected bool Equals(WorkDay other)
        {
            return _date.Equals(other._date);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((WorkDay)obj);
        }

        public override int GetHashCode()
        {
            return _date.GetHashCode();
        }

        public static implicit operator WorkDay(DateTime a)
        {
            return new WorkDay(a);
        }
        public static implicit operator DateTime(WorkDay a)
        {
            return a._date.Date;
        }
        /*
        public static bool operator ==(WorkDay a, WorkDay b)
        {
            if (a == null)
                return false;
            if (b == null)
                return false;
            return a._date == b._date;
        }

        public static bool operator !=(WorkDay a, WorkDay b)
        {
            return !(a == b);
        }*/

        public override string ToString()
        {
            return _date.ToShortDateString();
        }


        public int CompareTo(WorkDay other)
        {
            return _date.CompareTo(other._date);
        }
    }

}
