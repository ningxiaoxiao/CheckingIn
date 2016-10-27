using System;
using System.Collections.Generic;
using System.Data;

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
        public TimeSpan Holidays;

        /// <summary>
        /// 可调休时间/加班时间
        /// </summary>
        public TimeSpan OverWorkTime;

        //当月属性


        /// <summary>
        /// 出差天数
        /// </summary>
        public TimeSpan Travel = TimeSpan.Zero;
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
            var dv = new DataView(DB.Persons) { RowFilter = $"name ='{Name}'" };
            if (dv.Count == 0)
            {
                Mail = null;
                WorkTimeClass = WorkTimeClassInfo.Default;
                Log.Warn(n + "-无法得到个人信息");
            }
            else
            {

                Mail = dv[0]["mail"].ToString();


                var wtc = dv[0]["worktimeclass"].ToString();

                WorkTimeClass = new WorkTimeClassInfo(wtc);

                if (Mail == "")
                    Log.Warn(n + "-mail err");
                if (wtc == "")
                    Log.Warn(n + "-worktimeclass err");

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

                        if (t > WorkTimeClass.InTime)
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

        public CheckInfo GetCheck(WorkDay d)
        {

            foreach (var c in Checks)
            {
                if (c.Date == d)
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
    public class CheckInfo
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





                if (InTime > Person.WorkTimeClass.InTime)
                {

                    var k = (TimeSpan)(InTime - Person.WorkTimeClass.InTime);

                    Person.DelayTime += k;

                    _warns.Add(new WarnInfo(Date, $"迟到{k.TotalMinutes.ToString("####")}分钟"));
                }



                if (OutTime < Person.WorkTimeClass.OutTime)
                {
                    var k = (TimeSpan)(Person.WorkTimeClass.OutTime - OutTime);

                    Person.DelayTime += k;

                    _warns.Add(new WarnInfo(Date, $"早退{k.TotalMinutes.ToString("####")}分钟"));
                }

                return _warns;


            }
        }

        public bool HaveWarn => Warns.Count > 0;


        public DataView Sourcerec;

        public CheckInfo(PersonInfo p, WorkDay d, CheckTime it, CheckTime ot)
        {
            Person = p;
            Date = d;

            InTime = it;
            OutTime = ot;

        }
        public CheckInfo(PersonInfo p, WorkDay d)
            : this(p, d, null, null)
        {

        }

    }

    public class WorkDay
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
            return obj.GetType() == GetType() && Equals((WorkDay) obj);
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
        }

        public override string ToString()
        {
            return _date.ToShortDateString();
        }
    }

}
