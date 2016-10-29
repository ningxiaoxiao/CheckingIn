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

        public static DataTable OriginalListDt;

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
        }

        public static void Readpersondb()
        {
            PersonInfos = GetSql("select * from person");
        }

        public static void ResultDtAdd(string name, object dt, object intime, object outtime, out TimeSpan worktime)
        {
            var rr = DB.Resultdt.NewRow();
            rr["name"] = name;
            rr["Date"] = dt;
            rr["intime"] = intime;
            rr["outtime"] = outtime;
            var wt = (TimeSpan)outtime - (TimeSpan)intime;
            rr["worktime"] = wt;
            worktime = wt;
            DB.Resultdt.Rows.Add(rr);

            Insertdb("result", new[] { "name", "Date", "intime", "outtime", "worktime" }, new[] { name, dt, intime, outtime, wt });
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
            PersonInfos = new DataTable();

            PersonInfos.Columns.Add("name", typeof(string));
            PersonInfos.Columns.Add("mail", typeof(string));
            PersonInfos.Columns.Add("worktimeclass", typeof(string));

            //结果表
            Resultdt = new DataTable();

            Resultdt.Columns.Add("name", typeof(string));
            Resultdt.Columns.Add("Date", typeof(DateTime));
            Resultdt.Columns.Add("intime", typeof(TimeSpan));
            Resultdt.Columns.Add("outtime", typeof(TimeSpan));
            Resultdt.Columns.Add("worktime", typeof(TimeSpan));




            //二次处理的表

            OriginalDt = new DataTable();
            OriginalDt.Columns.Add("name", typeof(string));
            OriginalDt.Columns.Add("Date", typeof(DateTime));
            OriginalDt.Columns.Add("time", typeof(TimeSpan));
            OriginalDt.Columns.Add("info", typeof(string));




        }
        private static void CreatSqlTable()
        {
            Cmd("create table person (name varchar(20) primary key , mail varchar(50),worktimeclass varchar(20))");
            Cmd("create table result (name varchar(20), date date,intime time,outtime time,worktime time)");
            Cmd("create table warn (name varchar(20), date date,txt varchar(20))");
            Cmd("create table original (name varchar(20), date date,time time,info varchar(20))");
            Cmd("create table oa (no integer primary key,name varchar(20), start datetime,end datetime,reason varchar(20))");
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
        public static void Readoa()
        {
            var sql = "select * from oa order by no asc";
            OaDt = GetSql(sql);

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
        public static void AddOriginal(string name, DateTime date, TimeSpan t, string rs = null)
        {

            var r = OriginalDt.NewRow();
            r["name"] = name;
            r["Date"] = date.Date;
            r["time"] = t;
            r["info"] = rs;
            OriginalDt.Rows.Add(r);


            // Insertdb("original", new[] { "name", "Date", "time" }, new object[] { name, Date, t });

        }


        public static void Close()
        {
            _db.Close();
        }
        public static DataTable GetSql(string sql)
        {
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
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
