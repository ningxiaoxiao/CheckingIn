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
        /// 处理后的表格
        /// </summary>
        public static DataTable Resultdt;

        public static DataTable PersonInfos;
        /// <summary>
        /// 原始表格
        /// </summary>
        public static DataTable OriginalDt;


        public static DataTable OaDt;

        private static SQLiteConnection _db;

        /// <summary>
        /// 全公司的人
        /// </summary>
        public static Dictionary<string, PersonInfo> persons = new Dictionary<string, PersonInfo>();


        public static void Creat()
        {
            CreatDataTable();
            CheckSqlFile();

            Readoa();
            Readpersondb();
            ReadOriginalFormDB();
        }

        public static void Readpersondb()
        {
            PersonInfos = GetSql("select * from person");
            Log.Info("read person done");
        }
        public static void ReadOriginalFormDB()
        {
            GetSql("select * from original", OriginalDt);

            GetNamesAndDates();
            Log.Info("ReadOriginalFormDB done");
        }

        private static void GetNamesAndDates()
        {
            //得到所有有人出勤的日期
            var dv = new DataView(DB.OriginalDt);
            //读出所有姓名
            var _allnames = dv.ToTable(true, "name");

            //得到所有人出勤的日子
            var _alldates = dv.ToTable(true, "Date");

            //对所有有人出勤的日期进行遍历
            foreach (DataRow dater in _alldates.Rows)
            {
                //今日日期
                var date = dater["Date"];
                //判断是不是工作日
                //如果有30个出勤,就算工作日
                var dateview = new DataView(DB.OriginalDt) { RowFilter = $"date ='{date}'" }; //去重
                var pcount = dateview.ToTable(true, "name");

                WorkDay.AllDays.Add(((DateTime)date).Date, pcount.Rows.Count > 50);
            }

            //对每人个进行遍历
            foreach (DataRow r in _allnames.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();
                DB.persons.Add(n, new PersonInfo(n));

                CheckingIn.inst.comboBox1.Items.Add(n);
            }
            if (CheckingIn.inst.comboBox1.Items.Count > 0)
                CheckingIn.inst.comboBox1.SelectedIndex = 0;



        }
        public static void Readoa()
        {
            var sql = "select * from oa order by no asc";
            OaDt = GetSql(sql);
            Log.Info("Readoa done");
            //todo 处理oa表
        }
        public static void ResultDtAdd(string name, object dt, object intime, object outtime, string worktime, string info)
        {
            Insertdb("result", new[] { "name", "Date", "intime", "outtime", "worktime", "info" }, new[] { name, dt, intime, outtime, worktime, info });
        }
        public static TimeSpan GetOverWorkTimeCount(string name)
        {
            var t = new TimeSpan();
            var dv = new DataView(DB.OaDt) { RowFilter = $"name = '{name}' and reason ='加班'" };
            foreach (DataRowView dr in dv)
            {
                var s = (DateTime)dr.Row["start"];
                var ee = (DateTime)dr.Row["end"];
                t += ee - s;
            }
            return t;
        }
        public static int GetOutDaysCount(string name)
        {
            var t = new TimeSpan();
            var dv = new DataView(DB.OaDt) { RowFilter = $"name = '{name}' and reason ='出差'" };
            if (dv.Count == 0)
                return 0;
            foreach (DataRowView dr in dv)
            {
                var s = (DateTime)dr.Row["start"];
                var ee = (DateTime)dr.Row["end"];
                t += ee - s;
            }
            return t.Days + 1;
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
            Resultdt = new DataTable();

            Resultdt.Columns.Add("name", typeof(string));
            Resultdt.Columns.Add("Date", typeof(DateTime));
            Resultdt.Columns.Add("intime", typeof(TimeSpan));
            Resultdt.Columns.Add("outtime", typeof(TimeSpan));
            Resultdt.Columns.Add("worktime", typeof(TimeSpan));


            //原始数据表  //可以按这样的数据进行转换

            OriginalDt = new DataTable();
            OriginalDt.Columns.Add("name", typeof(string));
            OriginalDt.Columns.Add("Date", typeof(DateTime));
            OriginalDt.Columns.Add("time", typeof(TimeSpan));
            OriginalDt.Columns.Add("info", typeof(string));


            Log.Info("CreatDataTable done");

        }
        private static void CreatSqlTable()
        {
            Cmd("create table person (name varchar(20) primary key , mail varchar(50),worktimeclass varchar(20))");
            Cmd("create table result (name varchar(20), date date,intime time,outtime time,worktime time,info varchar(20))");
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

        public static void OaAdd(string name, DateTime s, DateTime e, string r)
        {
            var sql = $"insert into oa (name,start,end,reason) values ('{name}','{s.ToString("s")}','{e.ToString("s")}','{r}')";
            var ex = Cmd(sql);
        }

        /// <summary>
        /// 向处理后的;xls表增加数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="t"></param>
        /// <param name="rs"></param>
        public static void AddOriginal(string name, DateTime date, TimeSpan t, string rs = "")
        {
            /*
            var r = OriginalDt.NewRow();
            r["name"] = name;
            r["Date"] = date.Date;
            r["time"] = t;
            r["info"] = rs;
            OriginalDt.Rows.Add(r);
            */

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
