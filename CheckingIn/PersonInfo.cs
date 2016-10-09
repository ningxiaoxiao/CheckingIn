using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CheckingIn
{
    public class PersonInfo
    {
        //总属性
        public string name;

        /// <summary>
        /// 可用假期
        /// </summary>
        public TimeSpan holidays;

        /// <summary>
        /// 可调休时间/加班时间
        /// </summary>
        public TimeSpan overtime;





        //当月属性


        /// <summary>
        /// 出差天数
        /// </summary>
        public TimeSpan Travel = TimeSpan.Zero;
        /// <summary>
        /// 迟到早退时间
        /// </summary>
        public TimeSpan delayTime = TimeSpan.Zero;

        /// <summary>
        /// 异常天数
        /// </summary>
        public int wranDayCount = 0;

        /// <summary>
        /// 工作时间
        /// </summary>
        public TimeSpan WorkTime = TimeSpan.Zero;

        /// <summary>
        /// 工作时间班次
        /// </summary>
        public WorkTimeClassInfo WorkTimeClass { get; }

        private  List<CheckInfo> checks=new List<CheckInfo>(); 



        public PersonInfo(string n)
        {
            name = n;


            WorkTimeClass = GetWorkTimeClass(n);
        }

        public void AddCheck(CheckInfo c)
        {
            checks.Add(c);
        }

        private WorkTimeClassInfo GetWorkTimeClass(string name)
        {
            var cn = "早班";
            try
            {
                cn = DB._classTime[name];
            }
            catch (Exception)
            {

                Log.warn(name + "-不在班次表中");
            }

            return new WorkTimeClassInfo(cn);

        }


    }

    public struct WorkTimeClassInfo
    {
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
        public string ClassName;
        public TimeSpan InTime;
        public TimeSpan OutTime;
        public bool isWorkTimeClass => ClassName == "综合班次";
    }

    public struct WarnInfo
    {
        public WarnInfo(DateTime d, string i, WarnInfoType t)
        {
            date = d;
            info = i;
            type = t;
        }
        public WarnInfo(DateTime d, string i)
        {
            date = d;
            info = i;
            type = WarnInfoType.warn;
        }
        public DateTime date;
        public string info;
        public WarnInfoType type;
    }

    /// <summary>
    /// 打卡时间
    /// </summary>
    public class CheckTime
    {
        public CheckTime(TimeSpan t)
        {
            Time = t;
        }

        public CheckTime(string t)
        {
            Time = TimeSpan.Parse(t);
        }
        public TimeSpan Time { get; set; }

        public static bool operator ==(CheckTime a, CheckTime b)
        {
            try
            {
                return a.Time == b.Time;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool operator !=(CheckTime a, CheckTime b)
        {
            return !(a == b);
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

        public static implicit operator TimeSpan(CheckTime a)
        {
            return a.Time;
        }
        public static implicit operator CheckTime(TimeSpan a)
        {
            return new CheckTime(a);
        }
    }

    public enum WarnInfoType
    {
        info,
        warn
    }

    /// <summary>
    /// 打卡记录
    /// </summary>
    public class CheckInfo
    {
        public PersonInfo Person;

        public DateTime Date;

        public bool IsWorkDay { get; }

        public CheckTime InTime;
        public CheckTime OutTime;
        public TimeSpan WorkTime
        {
            get
            {
                try
                {
                    return OutTime - InTime;
                }
                catch (Exception)
                {

                    return TimeSpan.Zero;
                }
            }
        }

        public List<WarnInfo> Warns;


        public DataTable sourcerec;

        public CheckInfo(PersonInfo p, DateTime d, string it, string ot)
        {
            Date = d;
            try
            {
                InTime = new CheckTime(it);
                OutTime = new CheckTime(ot);
            }
            catch (Exception)
            {

                throw;
            }





            if (InTime == null && OutTime == null && IsWorkDay)
            {
                Warns.Add(new WarnInfo(d, "旷工"));
                return;
            }



            if (InTime == null)
            {
                Warns.Add(new WarnInfo(d, "上班未打卡"));
                return;
            }
            if (OutTime == null)
            {
                Warns.Add(new WarnInfo(d, "下班未打卡"));
                return;
            }

            //使用弹性工作制
            if (Person.WorkTimeClass.isWorkTimeClass)
                return;

            var k = TimeSpan.Zero;

            if (InTime > Person.WorkTimeClass.InTime)
            {
                //计算机动时间
                k = (TimeSpan)InTime - Person.WorkTimeClass.InTime;

                Person.delayTime += k;

                Warns.Add(new WarnInfo(d, $"迟到{k.TotalMinutes.ToString("####")}分钟"));
            }

            var shoudouttime = Person.WorkTimeClass.OutTime + k;

            if (OutTime < shoudouttime)
            {
                k = shoudouttime - (TimeSpan)OutTime;

                Person.delayTime += k;

                Warns.Add(new WarnInfo(d, $"早退{k.TotalMinutes.ToString("####")}分钟"));
            }
        }
    }

}
