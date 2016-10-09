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
        /// <summary>
        /// 原始表格
        /// </summary>
        public static DataTable Xlsdt;
        public static DataTable Listdt;

        public static DataTable OAdt;

        public static DataTable WarnTable;

        private static SQLiteConnection _db;

        public static Dictionary<string, string> _classTime = new Dictionary<string, string>();
        public static Dictionary<string, string> _mail = new Dictionary<string, string>();
        public static void Insertdb(string tablename, string[] k, object[] v)
        {

            var names = "";
            foreach (var i in k)
            {
                names += i + ",";
            }


            DB.dbcmd($"insert into {tablename} ({names.Substring(0, names.Length - 1)}) values ({v})");
        }
        public static void Opendb()
        {
            _db = new SQLiteConnection("Data Source=db.db");
            _db.Open();
        }

        public static void InstTable()
        {
            //结果表
            Resultdt = new DataTable();

            Resultdt.Columns.Add("name", typeof(string));
            Resultdt.Columns.Add("date", typeof(DateTime));
            Resultdt.Columns.Add("intime", typeof(TimeSpan));
            Resultdt.Columns.Add("outtime", typeof(TimeSpan));
            Resultdt.Columns.Add("worktime", typeof(TimeSpan));


            //警告表
            WarnTable = new DataTable();
            WarnTable.Columns.Add("name", typeof(string));
            WarnTable.Columns.Add("date", typeof(DateTime));
            WarnTable.Columns.Add("txt", typeof(string));


            //二次处理的表

            Xlsdt = new DataTable();

            Xlsdt.Columns.Add("name", typeof(string));
            Xlsdt.Columns.Add("date", typeof(DateTime));
            Xlsdt.Columns.Add("time", typeof(TimeSpan));


            //OA表
            OAdt = new DataTable();
            OAdt.Columns.Add("no", typeof(int));
            OAdt.Columns.Add("name", typeof(string));
            OAdt.Columns.Add("start", typeof(DateTime));
            OAdt.Columns.Add("end", typeof(DateTime));
            OAdt.Columns.Add("reason", typeof(string));

        }
        public static void NewdbTable()
        {
            dbcmd("create table mail (name varchar(20), mail varchar(50))");
            dbcmd("create table classtime (name varchar(20), Classname varchar(20))");
            dbcmd("create table result (name varchar(20), date date,intime time,outtime time,worktime time)");
            dbcmd("create table warn (name varchar(20), date date,txt varchar(20))");
            dbcmd("create table xls (name varchar(20), date date,time time)");
            dbcmd("create table oa (no integer primary key,name varchar(20), start datetime,end datatime,reason varchar(20))");

        }
        public static void Checkdbfile()
        {
            if (!File.Exists("db.db"))
            {
                SQLiteConnection.CreateFile("db.db");

                Opendb();
                NewdbTable();
            }
            else
            {
                Opendb();
            }

        }
        public static void Readoa()
        {
            var sql = "select * from oa order by no asc";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();
            OAdt.Clear();
            OAdt.Load(reader);
        }

        public static void Readmaildb()
        {
            var sql = "select * from mail";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var name = reader["name"].ToString();
                var m = reader["mail"].ToString();

                if (!_mail.ContainsKey(name))
                    _mail.Add(name, m);
            }

        }

        public static void ReadClassTimeFormDB()
        {
            //从表中得到数据

            var sql = "select * from classtime";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _classTime.Add(reader["name"].ToString(), reader["Classname"].ToString());
            }

        }
        public static int dbcmd(string cmd, SQLiteTransaction tran = null)
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
            var ex = dbcmd(sql);
        }

        /// <summary>
        /// 向处理后的;xls表增加数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="t"></param>
        /// <param name="rs"></param>
        public static void xlsadd(string name, DateTime date, TimeSpan t, string rs = null)
        {
            var r = Xlsdt.NewRow();
            r["name"] = name;
            r["date"] = date.Date;
            r["time"] = t;
            Xlsdt.Rows.Add(r);

            if (rs != null)
            {
                Warn(name, date, rs);
            }

        }
        /// <summary>
        /// 异常警告增加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        /// <param name="t"></param>
        public static void Warn(string name, object dt, string t)
        {
            var wr = WarnTable.NewRow();
            wr["name"] = name;
            wr["date"] = ((DateTime)dt).Date;
            wr["txt"] = t;
            WarnTable.Rows.Add(wr);
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
