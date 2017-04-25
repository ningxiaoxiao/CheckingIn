using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LitJson2;

namespace CheckingIn
{

    public class PersonInfo
    {
        private DateTime _lastGetTime { get; set; }

        //总属性
        public string Name;

        public string Mail;

        /// <summary>
        /// 工作时间班次
        /// </summary>
        public WorkTimeClassInfo WorkTimeClass;

        /// <summary>
        /// 剩余假期
        /// </summary>
        public TimeSpan CanUseHolidayHour = TimeSpan.Zero;

        //当月属性

        /// <summary>
        /// 出差天数
        /// </summary>
        public int Travel;

        private int _warnDayCount = -1;

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

        /// <summary>
        /// 迟到早退时间
        /// </summary>
        public TimeSpan DelayTime = TimeSpan.Zero;

        /// <summary>
        /// 应出勤天数
        /// </summary>
        public int ShoudWorkDayCount;

        /// <summary>
        /// 工作时间
        /// </summary>
        public TimeSpan WorkTime = TimeSpan.Zero;


        public List<CheckInfo> Checks = new List<CheckInfo>();



        /// <summary>
        /// 加班时间
        /// </summary>
        public TimeSpan OverWorkTime;
        /// <summary>
        /// 使用假使用
        /// </summary>
        public TimeSpan useHolidayhours;
        /// <summary>
        /// 扣薪假期
        /// </summary>
        public TimeSpan NoPayHolidaysHours;

        private readonly List<DateTime> AllDays = new List<DateTime>();

        public PersonInfo(string n)
        {
            Name = n;
            //从db中得到相关信息
            var p = DB.Context.From<Dos.Model.person>().Where(d => (d.name == Name)).First();

            if (p == null)
            {
                //没有找到这个人相关数据

                Mail = null;
                WorkTimeClass = WorkTimeClassInfo.Default;
                Log.Err(n + "-无法得到个人信息");
            }
            else
            {

                Mail = p.mail;
                WorkTimeClass = new WorkTimeClassInfo(p.worktimeclass);

                if (Mail == "")
                    Log.Err(n + "-mail err");
                if (p.worktimeclass == "")
                    Log.Err(n + "-worktimeclass err");

            }

        }

        public void AddCheck(CheckInfo c)
        {
            Checks.Add(c);
            WorkTime += c.WorkTime;
        }
        private static void CheckDTAdd(string name, DateTime date, TimeSpan t, string i, DataTable dt)
        {
            var r = dt.NewRow();
            r["name"] = name;
            r["date"] = date;
            r["time"] = t.Ticks;
            r["info"] = i;
            dt.Rows.Add(r);
        }
        private void SetMonth(int m)
        {
            AllDays.Clear();
            //得到本月所有的日期
            var n = new DateTime(DateTime.Now.Year, m, 1);
            while (n.Month == m)
            {
                AllDays.Add(n);
                n = n.AddDays(1);
            }
        }


        private bool isworkday(DateTime dt)
        {
            var str = dt.Month.ToString("00") + dt.Day.ToString("00");

            return !CheckingIn.Inst.workdaysjson.Keys.Contains(str);
        }

        private void reset()
        {
            Checks.Clear();
            OverWorkTime = new TimeSpan();
            Travel = 0;
            NoPayHolidaysHours = new TimeSpan();
            useHolidayhours = new TimeSpan();
            DelayTime = new TimeSpan();
            ShoudWorkDayCount = 0;
            _warnDayCount = -1;
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="month">月份</param>
        public void GetData(int month)
        {

            reset();


            SetMonth(month);




            var AllCheckDT = DB.Context.From<Dos.Model.original>().Where(d => d.name == Name).ToDataTable();
            var AlloaDT = DB.Context.From<Dos.Model.oa>().Where(d => d.name == Name).ToDataTable();



            //遍历所有日期
            foreach (var date in AllDays)
            {

                //更新前天的数据

                if (date >= DateTime.Today.AddDays(-1))
                    break;





                var willaddcheck = new CheckInfo(this, date);
                if (isworkday(date))
                    ShoudWorkDayCount++;
                else
                {
                    willaddcheck.Warns.Add(new WarnInfo(date, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek), WarnInfoType.Info));
                }


                // 先处理OA
                //得到这个人今天的oa数据
                var oadata = new DataView(AlloaDT)
                {
                    RowFilter = $"date = '{date}'",
                };

                ProssOA(AllCheckDT, oadata);

                //得到这个人今天所有的打卡时间
                var checkDT = new DataView(AllCheckDT)
                {
                    RowFilter = $"date = '{date}'",
                }.ToTable();



                var data = new DataView(checkDT)
                {
                    Sort = "time asc" //从小到大
                };
                willaddcheck.Sourcerec = data;
                //合成信息
                foreach (var r in from DataRowView t in data select t["info"].ToString() into r where !willaddcheck.Info.Contains(r) select r)
                {
                    willaddcheck.Info += r + " ";
                }

                //得到上下班时间
                switch (data.Count)//这一天打卡次数
                {
                    case 0:
                        break;
                    case 1:
                        //判断是上班,还是下班

                        var t = new TimeSpan((long)data[0].Row["time"]);


                        if (WorkTimeClass.IsWorkTimeClass)
                        {
                            willaddcheck.InTime = t;
                        }
                        else
                        {
                            //大于上班时间4小时
                            if (t > WorkTimeClass.InTime + new TimeSpan(0, 4, 0, 0))
                                willaddcheck.OutTime = t;
                            else
                                willaddcheck.InTime = t;
                        }

                        break;

                    default:
                        //2次及2次以上 
                        //取第一个 和最后一个

                        var t1 = new TimeSpan((long)data[0].Row["time"]);
                        var t2 = new TimeSpan((long)data[data.Count - 1].Row["time"]);

                        //综合工时直接计算
                        if (WorkTimeClass.IsWorkTimeClass)
                        {
                            willaddcheck.InTime = t1;
                            willaddcheck.OutTime = t2;
                            break;
                        }


                        willaddcheck.InTime = t1;
                        willaddcheck.OutTime = t2;
                        break;
                }
                AddCheck(willaddcheck);
            }

            Checks.Sort();


        }

        private void ProssOA(DataTable checkDT, DataView oadata)
        {
            //处理oa数据

            foreach (DataRowView drv in oadata)
            {
                var reason = drv["reason"].ToString();
                var subreason = drv["subreason"].ToString();

                switch (reason)
                {
                    case "加班":
                        //累加到加班时间中去

                        var st1 = (DateTime)drv["start"];
                        var et1 = (DateTime)drv["end"];

                        //取整小时
                        var owt = et1 - st1;
                        OverWorkTime += new TimeSpan(owt.Days, owt.Hours, 0, 0);

                        //模拟两次打卡

                        CheckDTAdd(Name, st1.Date, st1.TimeOfDay, reason + "开始", checkDT);
                        CheckDTAdd(Name, et1.Date, et1.TimeOfDay, reason + "结束", checkDT);

                        break;
                    case "外出":

                        var st = (DateTime)drv["start"];
                        var et = (DateTime)drv["end"];

                        //模拟打卡
                        CheckDTAdd(Name, st.Date, st.TimeOfDay, reason + "开始", checkDT);
                        CheckDTAdd(Name, et.Date, et.TimeOfDay, reason + "结束", checkDT);




                        var ds = et.Day - st.Day;


                        for (var j = 0; j < ds; j++)
                        {
                            var c = st.Date + new TimeSpan(j, 0, 0, 0) + WorkTimeClass.InTime;//上班时间

                            if (c > st && c < et)//如果在相隔时间内,加一次打卡
                                CheckDTAdd(Name, c.Date, c.TimeOfDay, "外出中", checkDT);

                            c = st.Date + new TimeSpan(j, 0, 0, 0) + WorkTimeClass.OutTime;//下班时间

                            if (c > st && c < et)//
                                CheckDTAdd(Name, c.Date, c.TimeOfDay, "外出中", checkDT);

                        }
                        break;
                    case "补登":

                        CheckDTAdd(Name,
                            (DateTime)drv["start"],
                            subreason == "上班" ? WorkTimeClass.InTime : WorkTimeClass.OutTime,
                            reason,
                            checkDT);

                        break;
                    case "出差":
                        //出差期间,每天自动增加一个上班打卡 和下班打卡
                        var s = (DateTime)drv["start"];
                        var ee = (DateTime)drv["end"];

                        //累加出差天数
                        Travel += ee.Day - s.Day + 1;


                        //先增加开始和结束
                        CheckDTAdd(Name, s, WorkTimeClass.InTime, reason + "开始", checkDT);
                        CheckDTAdd(Name, ee, WorkTimeClass.OutTime, reason + "结束", checkDT);

                        //去掉时间
                        s = s.Date;
                        ee = ee.Date;

                        //得到出差几天
                        var days = ee - s;

                        for (var d = 0; d <= days.Days; d++)
                        {
                            CheckDTAdd(Name, s + new TimeSpan(d, 0, 0, 0), (TimeSpan)WorkTimeClass.InTime, "出差中", checkDT);
                            CheckDTAdd(Name, s + new TimeSpan(d, 0, 0, 0), (TimeSpan)WorkTimeClass.OutTime, "出差中", checkDT);
                        }

                        break;
                    case "休假":


                        //模拟打卡

                        var st2 = (DateTime)drv["start"];
                        var et2 = (DateTime)drv["end"];

                        var time = et2 - st2;

                        CheckDTAdd(Name, st2.Date, st2.TimeOfDay, subreason + "开始", checkDT);
                        CheckDTAdd(Name, et2.Date, et2.TimeOfDay, subreason + "结束", checkDT);

                        switch (subreason)
                        {
                            case "事假":
                            case "病假":
                            case "公伤假":
                                NoPayHolidaysHours += time;
                                break;
                            default:
                                useHolidayhours += time;
                                break;
                        }


                        break;

                }
            }
        }

        public JsonData GetJson()
        {
            var j = new JsonData();

            var csjs = new JsonWriter();

            csjs.WriteArrayStart();


            foreach (var c in Checks)
            {

                csjs.WriteObjectStart();

                csjs.WritePropertyName("date");
                csjs.Write(c.Date.ToString());

                csjs.WritePropertyName("info");
                csjs.Write(c.Info);

                csjs.WritePropertyName("warninfo");
                csjs.Write(c.warninfo);

                csjs.WritePropertyName("intime");
                csjs.Write(c.InTime.ToMyString());

                csjs.WritePropertyName("outtime");
                csjs.Write(c.OutTime.ToMyString());

                csjs.WriteObjectEnd();

            }


            csjs.WriteArrayEnd();


            var profile = new JsonData();

            profile["姓名"] = Name;
            profile["剩余假期"] = CanUseHolidayHour.TotalHours.ToString("0.#") + "小时";
            profile["工作班次"] = WorkTimeClass.ToString();


            if (WorkTimeClass.IsWorkTimeClass)
                profile["工作时间"] = WorkTime.TotalHours.ToString("0.#") + "/" + ShoudWorkDayCount * 8 + "小时";
            else
            {
                profile["工作天数"] = ShoudWorkDayCount - WarnDayCount + "/" + ShoudWorkDayCount + "天";
                profile["异常天数"] = WarnDayCount + "天";
                profile["迟到早退"] = DelayTime.TotalMinutes.ToString("0.# '分钟'");
            }

            profile["使用调休假期"] = useHolidayhours.TotalHours.ToString("0.#") + "小时";
            profile["使用扣薪假期"] = NoPayHolidaysHours.TotalHours.ToString("0.#") + "小时";
            profile["加班"] = OverWorkTime.TotalHours.ToString("0.# '小时'");
            profile["出差"] = Travel + "天";

            j["profile"] = profile;

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

        public override string ToString()
        {
            return Name;
        }

        internal void Reset()
        {
            _warnDayCount = -1;
            _geted = false;
            Travel = 0;
            DelayTime = TimeSpan.Zero;
            OverWorkTime = new TimeSpan(-1);
        }

        private bool _geted;
    }
    /// <summary>
    /// 工作班次信息
    /// </summary>
    public class WorkTimeClassInfo
    {
        public string ClassName;
        public TimeSpan InTime;
        public TimeSpan OutTime;
        public bool IsWorkTimeClass => ClassName == "综合班次";
        public static WorkTimeClassInfo Default = new WorkTimeClassInfo("早班");



        public WorkTimeClassInfo(string n)
        {
            ClassName = n;

            var j = CheckingIn.Inst.worktimeclassjson[n];

            InTime = TimeSpan.Parse(j["in"].ToString());
            OutTime = TimeSpan.Parse(j["out"].ToString());

        }

        public override string ToString()
        {
            return ClassName + "|" + InTime + "|" + OutTime;
        }
    }

    /// <summary>
    /// 打卡记录
    /// </summary>
    public class CheckInfo : IComparable<CheckInfo>
    {
        public readonly static TimeSpan UnKownTimeSpan = new TimeSpan(-1);
        public PersonInfo Person;

        public WorkDay Date;



        public TimeSpan InTime = UnKownTimeSpan;
        public TimeSpan OutTime = UnKownTimeSpan;

        public TimeSpan WorkTime => ((OutTime != UnKownTimeSpan && InTime != UnKownTimeSpan) ? OutTime - InTime : TimeSpan.Zero);


        private List<WarnInfo> _warns;
        public List<WarnInfo> Warns
        {
            get
            {
                if (_warns != null) return _warns;

                _warns = new List<WarnInfo>();
                //不是工作日直接返回
                if (!Date.IsWorkDay) return _warns;

                //弹性工作制警告消息                                                      
                if (Person.WorkTimeClass.IsWorkTimeClass)
                {
                    if (InTime == UnKownTimeSpan && OutTime == UnKownTimeSpan)
                        return _warns;

                    //是不是单次打卡
                    if (InTime == UnKownTimeSpan || OutTime == UnKownTimeSpan)
                        _warns.Add(new WarnInfo(Date, "单次打卡", WarnInfoType.Info));

                    return _warns;

                }

                //都是空且,还是上班
                if (InTime == UnKownTimeSpan && OutTime == UnKownTimeSpan && Date.IsWorkDay)
                {
                    _warns.Add(new WarnInfo(Date, "旷工"));
                    return _warns;
                }


                if (InTime == UnKownTimeSpan)
                {
                    _warns.Add(new WarnInfo(Date, "上班未打卡"));
                    return _warns;
                }
                if (OutTime == UnKownTimeSpan)
                {
                    _warns.Add(new WarnInfo(Date, "下班未打卡"));
                    return _warns;
                }

                //到这里 就是有两次合法打卡 计算迟到时间

                var delayTime = TimeSpan.Zero;

                if (InTime > Person.WorkTimeClass.InTime)
                {

                    delayTime = InTime - Person.WorkTimeClass.InTime;
                    //机动时间
                    var extime = new TimeSpan(0, 0, 5, 0);
                    if (delayTime > extime)
                    {

                        var dt = delayTime - extime;//迟到时间
                        if (dt.TotalMinutes < 120)//2个小时
                        {
                            Person.DelayTime += dt;
                            _warns.Add(new WarnInfo(Date, $"迟到{dt.TotalMinutes.ToString("0.#")}分钟"));
                        }
                        else
                        {
                            //迟到>2小时.直接算未打卡
                            _warns.Add(new WarnInfo(Date, $"上班未打卡(超时),迟到{dt.TotalMinutes.ToString("0.#")}分钟"));

                        }

                        delayTime = extime;


                    }


                }

                var shoudout = Person.WorkTimeClass.OutTime + delayTime;

                if (OutTime < shoudout)
                {
                    delayTime = shoudout - OutTime;


                    if (delayTime.TotalMinutes < 120)
                    {
                        Person.DelayTime += delayTime;
                        _warns.Add(new WarnInfo(Date, $"早退{delayTime.TotalMinutes.ToString("0.#")}分钟"));

                    }
                    else
                    {
                        _warns.Add(new WarnInfo(Date, $"下班未打卡(超时),早退{delayTime.TotalMinutes.ToString("0.#")}分钟"));
                    }



                }

                return _warns;


            }
        }

        public bool HaveWarn
        {
            get
            {
                var w = Warns.Find(d => d.Type == WarnInfoType.Warn);
                return w != null;
            }
        }



        public string Info;
        public DataView Sourcerec;

        internal string warninfo
        {
            get
            {
                return Warns.Aggregate("", (current, w) => current + (w.Info + " "));
            }
        }

        public CheckInfo(PersonInfo p, WorkDay d)
        {
            Person = p;
            Date = d;

            InTime = UnKownTimeSpan;
            OutTime = UnKownTimeSpan;
            Info = "";
        }



        public int CompareTo(CheckInfo other)
        {
            return Date.CompareTo(other.Date);
        }
    }

    public class WorkDay : IComparable<WorkDay>
    {


        public bool IsWorkDay
        {
            get
            {
                var str = _date.Month.ToString("00") + _date.Day.ToString("00");

                return !CheckingIn.Inst.workdaysjson.Keys.Contains(str);
            }
        }

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

        public override string ToString()
        {
            return _date.ToShortDateString();
        }

        public string ToString(string s)
        {
            return _date.ToString(s);
        }


        public int CompareTo(WorkDay other)
        {
            return _date.CompareTo(other._date);
        }
    }

    public class WarnInfo
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

    public enum WarnInfoType
    {
        Info,
        Warn
    }

}
