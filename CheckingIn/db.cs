using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace CheckingIn
{
    public static class DB
    {
        /// <summary>
        /// 处理后的表格 check +oa
        /// </summary>
        public static DataTable OaResults;

        public static DataTable PersonInfos;
        /// <summary>
        /// chick 原始表格
        /// </summary>
        public static DataTable CheckOriginalDt;


        public static DataTable OaOriginaDt;

        private static SQLiteConnection _db;

        /// <summary>
        /// 全公司的人
        /// </summary>
        public static Dictionary<string, PersonInfo> Persons = new Dictionary<string, PersonInfo>();


        public static void Creat()
        {
            CreatDataTable();
            CheckSqlFile();

            Readoa();
            ReadPersonDb();
            ReadOriginalFormDb();
        }

        public static void ReadPersonDb()
        {
            PersonInfos = GetSql("select * from person");
            Log.Info("read person done");
        }

        internal static void DelOrigina()
        {
            Cmd("delete  from original");
            ReadOriginalFormDb();
            Log.Info("delete original");
        }
        public static void ReadOriginalFormDb()
        {
            GetSql("select * from original", CheckOriginalDt);
            //得到所有有人出勤的日期
            var dv = new DataView(DB.CheckOriginalDt);
            //读出所有姓名
            var _allnames = dv.ToTable(true, "name");

            //得到所有人出勤的日子
            var _alldates = dv.ToTable(true, "date");

            //对所有有人出勤的日期进行遍历
            WorkDay.AllDays.Clear();
            foreach (DataRow dater in _alldates.Rows)
            {
                //今日日期
                var date = dater["date"];
                //判断是不是工作日
                //如果有30个出勤,就算工作日
                var dateview = new DataView(DB.CheckOriginalDt) { RowFilter = $"date ='{date}'" }; //去重
                var pcount = dateview.ToTable(true, "name");

                WorkDay.AllDays.Add(((DateTime)date).Date, pcount.Rows.Count > 50);
            }

            //生成所有人
            Persons.Clear();
            CheckingIn.inst.comboBox1.Items.Clear();
            foreach (DataRow r in _allnames.Rows)
            {
                var n = r["name"].ToString();

                Persons.Add(n, new PersonInfo(n));
                CheckingIn.inst.comboBox1.Items.Add(n);

            }
            if (CheckingIn.inst.comboBox1.Items.Count > 0)
                CheckingIn.inst.comboBox1.SelectedIndex = 0;

            Log.Info("ReadOriginalFormDb done");
        }

        public static void Readoa()
        {
            OaOriginaDt = GetSql("select * from oa order by no asc");
            OaResults.Clear();
            //读出原始OA数据
            foreach (DataRow i in DB.OaOriginaDt.Rows)
            {
                var reason = i["reason"].ToString();
                var name = i["name"].ToString();

                if (!DB.Persons.ContainsKey(name))
                    DB.Persons.Add(name, new PersonInfo(name));
                var p = DB.Persons[name];//得到人
                switch (reason)
                {
                    case "加班":
                        //直接计算时间

                        var st1 = (DateTime)i["start"];
                        var et1 = (DateTime)i["end"];

                        //模拟两次打卡

                        DB.OaResultAdd(name, st1.Date, st1.TimeOfDay, reason + "开始");
                        DB.OaResultAdd(name, et1.Date, et1.TimeOfDay, reason + "结束");

                        break;
                    case "外出":

                        var st = (DateTime)i["start"];
                        var et = (DateTime)i["end"];


                        DB.OaResultAdd(name, (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, reason + "开始");
                        DB.OaResultAdd(name, (DateTime)i["end"], ((DateTime)i["end"]).TimeOfDay, reason + "结束");

                        var ds = (int)(et - st).TotalDays;//得到相隔天数


                        for (var j = 0; j < ds; j++)
                        {
                            var c = st.Date + new TimeSpan(j, 0, 0, 0) + (TimeSpan)p.WorkTimeClass.InTime;//上班时间

                            if (c > st && c < et)//如果在相隔时间内,加一次打卡
                                DB.OaResultAdd(name, c.Date, c.TimeOfDay, reason);

                            c = st.Date + new TimeSpan(j, 0, 0, 0) + (TimeSpan)p.WorkTimeClass.OutTime;

                            if (c > st && c < et)//
                                DB.OaResultAdd(name, c.Date, c.TimeOfDay, reason);

                        }


                        break;
                    case "补登":

                        DB.OaResultAdd(name, (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, reason);

                        break;
                    case "出差":
                        //出差期间,每天自动增加一个上班打卡 和下班打卡
                        var s = (DateTime)i["start"];
                        var ee = (DateTime)i["end"];

                        //先增加开始和结束
                        DB.OaResultAdd(name, s, (TimeSpan)p.WorkTimeClass.InTime, reason + "开始");
                        DB.OaResultAdd(name, ee, (TimeSpan)p.WorkTimeClass.OutTime, reason + "结束");

                        //去掉时间
                        s = s.Date;
                        ee = ee.Date;

                        //得到出差几天
                        var days = ee - s;

                        for (var d = 0; d <= days.Days; d++)
                        {
                            DB.OaResultAdd(name, s + new TimeSpan(d, 0, 0, 0), (TimeSpan)p.WorkTimeClass.InTime, reason);
                            DB.OaResultAdd(name, s + new TimeSpan(d, 0, 0, 0), (TimeSpan)p.WorkTimeClass.OutTime, reason);
                        }

                        break;
                    case "请假":
                        break;
                    case "事假":

                        break;
                    case "调休":

                        break;
                }
            }
            Log.Info("Readoa done");
        }

        
        public static void Insertdb(string tablename, string[] k, object[] v)
        {

            var names = "";
            foreach (var i in k)
            {
                names += i + ",";
            }

            var vs = "";
            foreach (var i in v)
            {
                vs += "'" + i + "',";
            }

            var s = $"insert into {tablename} ({names.Substring(0, names.Length - 1)}) values ({vs.Substring(0, vs.Length - 1)})";

            Cmd(s);
        }
        private static void Opendb()
        {
            _db = new SQLiteConnection("Data Source=db.db");
            _db.Open();
        }

        private static void CreatDataTable()
        {
            //结果表
            OaResults = new DataTable();
            OaResults.Columns.Add("name", typeof(string));
            OaResults.Columns.Add("date", typeof(DateTime));
            OaResults.Columns.Add("time", typeof(TimeSpan));
            OaResults.Columns.Add("info", typeof(string));


            //原始数据表  //可以按这样的数据进行转换

            CheckOriginalDt = new DataTable();
            CheckOriginalDt.Columns.Add("name", typeof(string));
            CheckOriginalDt.Columns.Add("date", typeof(DateTime));
            CheckOriginalDt.Columns.Add("time", typeof(TimeSpan));
            CheckOriginalDt.Columns.Add("info", typeof(string));


            Log.Info("CreatDataTable done");

        }
        private static void CreatSqlTable()
        {
            Cmd("create table person (name varchar(20) primary key , mail varchar(50),worktimeclass varchar(20))");
            Cmd("create table checks (name varchar(20), date date,intime INTEGER,outtime INTEGER,worktime INTEGER,info varchar(20))");
            Cmd("create table warn (name varchar(20), date date,txt varchar(20))");
            Cmd("create table original (name varchar(20), date datetime,time INTEGER,info varchar(20))");
            Cmd("create table oa (no integer primary key,name varchar(20), start datetime,end datetime,reason varchar(20))");
            Log.Info("CreatSqlTable done");
        }
        private static void CheckSqlFile()
        {
            if (!File.Exists("db.db"))
            {
                SQLiteConnection.CreateFile("db.db");

                Opendb();
                CreatSqlTable();
            }
            else
            {
                Opendb();
            }

        }

        private static SQLiteTransaction _tran;

        public static int Cmd(string cmd)
        {

            var command = new SQLiteCommand(cmd, _db);
            if (_tran != null)
            {
                command.Transaction = _tran;
            }

            return command.ExecuteNonQuery();
        }



        public static void OaOriginaAdd(string name, DateTime s, DateTime e, string r)
        {
            var sql = $"insert into oa (name,start,end,reason) values ('{name}','{s.ToString("s")}','{e.ToString("s")}','{r}')";
            var ex = Cmd(sql);
        }

        public static void OaResultAdd(string name, DateTime date, TimeSpan t, string i)
        {
            var r = OaResults.NewRow();
            r["name"] = name;
            r["date"] = date;
            r["time"] = t;
            r["info"] = i;
            OaResults.Rows.Add(r);
        }

        /// <summary>
        /// 向处理后的;xls表增加数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="t"></param>
        /// <param name="rs"></param>
        public static void AddCheckOriginal(string name, DateTime date, TimeSpan t, string rs)
        {

            Insertdb("original", new[] { "name", "date", "time", "info" }, new object[] { name, date.Date.ToString("s"), t.Ticks, rs });
        }

        public static void Close()
        {
            _db.Close();
        }
        public static DataTable GetSql(string sql, DataTable sdt = null)
        {
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();

            if (sdt == null)
            {
                var dt = new DataTable();

                dt.Load(reader);
                return dt;
            }
            else
            {
                sdt.Load(reader);
                return sdt;
            }


        }

        public static void BeginTransaction()
        {
            _tran = _db.BeginTransaction();
        }

        public static void Commit()
        {
            _tran.Commit();
            _tran = null;
        }
        public static void Rollback()
        {
            _tran.Rollback();
            _tran = null;
        }
    }
}
