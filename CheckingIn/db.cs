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

        public static DataTable Persons;
        /// <summary>
        /// 原始表格
        /// </summary>
        public static DataTable OriginalDt;

        public static DataTable OriginalListDt;

        public static DataTable OaDt;

        public static DataTable WarnDt;

        private static SQLiteConnection _db;

        public static void Creat()
        {
            CreatDataTable();
            CheckSqlFile();

            Readoa();
            Readpersondb();
        }

        public static void Readpersondb()
        {
            Persons = GetSql("select * from person");
        }

        public static void ResultDtAdd(string name, object dt, object intime, object outtime, out TimeSpan worktime)
        {
            var rr = DB.Resultdt.NewRow();
            rr["name"] = name;
            rr["date"] = dt;
            rr["intime"] = intime;
            rr["outtime"] = outtime;
            var wt = (TimeSpan)outtime - (TimeSpan)intime;
            rr["worktime"] = wt;
            worktime = wt;
            DB.Resultdt.Rows.Add(rr);

            Insertdb("result", new[] { "name", "date", "intime", "outtime", "worktime" }, new[] { name, dt, intime, outtime, wt });
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
            Persons = new DataTable();

            Persons.Columns.Add("name", typeof(string));
            Persons.Columns.Add("mail", typeof(string));
            Persons.Columns.Add("worktimeclass", typeof(string));

            //结果表
            Resultdt = new DataTable();

            Resultdt.Columns.Add("name", typeof(string));
            Resultdt.Columns.Add("date", typeof(DateTime));
            Resultdt.Columns.Add("intime", typeof(TimeSpan));
            Resultdt.Columns.Add("outtime", typeof(TimeSpan));
            Resultdt.Columns.Add("worktime", typeof(TimeSpan));


            //警告表
            WarnDt = new DataTable();
            WarnDt.Columns.Add("name", typeof(string));
            WarnDt.Columns.Add("date", typeof(DateTime));
            WarnDt.Columns.Add("txt", typeof(string));


            //二次处理的表

            OriginalDt = new DataTable();
            OriginalDt.Columns.Add("name", typeof(string));
            OriginalDt.Columns.Add("date", typeof(DateTime));
            OriginalDt.Columns.Add("time", typeof(TimeSpan));


            //OA表
            OaDt = new DataTable();
            OaDt.Columns.Add("no", typeof(int));
            OaDt.Columns.Add("name", typeof(string));
            OaDt.Columns.Add("start", typeof(DateTime));
            OaDt.Columns.Add("end", typeof(DateTime));
            OaDt.Columns.Add("reason", typeof(string));

        }
        private static void CreatSqlTable()
        {
            Cmd("create table person (name varchar(20) primary key , mail varchar(50),worktimeclass varchar(20))");
            Cmd("create table result (name varchar(20), date date,intime time,outtime time,worktime time)");
            Cmd("create table warn (name varchar(20), date date,txt varchar(20))");
            Cmd("create table original (name varchar(20), date date,time time)");
            Cmd("create table oa (no integer primary key,name varchar(20), start datetime,end datatime,reason varchar(20))");
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
        private static void Readoa()
        {
            var sql = "select * from oa order by no asc";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();
            OaDt.Clear();
            OaDt.Load(reader);
        }

        public static int Cmd(string cmd, SQLiteTransaction tran = null)
        {

            var command = new SQLiteCommand(cmd, _db);
            if (tran != null)
            {
                command.Transaction = tran;
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
            r["date"] = date.Date;
            r["time"] = t;
            OriginalDt.Rows.Add(r);

            if (rs != null)
            {
                AddWarn(name, date, rs);
            }

            // Insertdb("original", new[] { "name", "date", "time" }, new object[] { name, date, t });

        }
        /// <summary>
        /// 异常警告增加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        /// <param name="t"></param>
        public static void AddWarn(string name, object dt, string t)
        {

            var wr = WarnDt.NewRow();
            wr["name"] = name;
            wr["date"] = ((DateTime)dt).Date;
            wr["txt"] = t;
            WarnDt.Rows.Add(wr);

            // Insertdb("warn", new[] { "name", "date", "txt" }, new object[] { name, ((DateTime)dt).Date, t });

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

        public static SQLiteTransaction BeginTransaction()
        {
            return _db.BeginTransaction();
        }
    }
}
