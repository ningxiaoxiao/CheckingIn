using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dos.ORM;

namespace CheckingIn
{
    public static class DB
    {

        public static readonly DbSession Context = new DbSession(DatabaseType.Sqlite3, "Data Source=db.db");

        /// <summary>
        /// oa的原始表
        /// </summary>
        public static DataTable OaOriginaDt;

        /// <summary>
        /// 处理后的oa数据表格
        /// </summary>
       // public static DataTable OaResults;
        /// <summary>
        /// 员工表
        /// </summary>
        public static DataTable PersonInfos;
        /// <summary>
        /// 全公司的人
        /// </summary>
        public static Dictionary<string, PersonInfo> Persons = new Dictionary<string, PersonInfo>();


        /// <summary>
        /// check 原始表格
        /// </summary>
        public static DataTable CheckOriginalDt;



        private static SQLiteConnection _db;

        private static SQLiteTransaction _tran;

        public static void Creat()
        {
            
            CreatDataTable();
            CheckSqlFile();
            /*
            Readoa();
            ReadPersonDb();
            ReadOriginalFormDb();*/
        }

        public static void ReadPersonDb()
        {
            PersonInfos = GetSql("select * from person");
            Log.Info("read person done");
        }

        public static void DelOrigina()
        {
            Cmd("delete  from original");
            ReadOriginalFormDb();
            Log.Info("delete original");
        }
        public static void DelOA()
        {
            Cmd("delete from oa");
            Log.Info("delete oa");
        }
        public static void ReadOriginalFormDb()
        {
            GetSql("select * from original", CheckOriginalDt);


            //得到所有有人出勤的日期
            var dv = new DataView(CheckOriginalDt);
            //读出所有姓名
            var _allnames = dv.ToTable(true, "name");

            //得到所有人出勤的日子
            var _alldates = dv.ToTable(true, "date");


            //生成所有人
            Persons.Clear();
            CheckingIn.Inst.comboBox1.Items.Clear();
            foreach (DataRow r in _allnames.Rows)
            {
                var n = r["name"].ToString();

                Persons.Add(n, new PersonInfo(n));
                CheckingIn.Inst.comboBox1.Items.Add(n);

            }
            if (CheckingIn.Inst.comboBox1.Items.Count > 0)
                CheckingIn.Inst.comboBox1.SelectedIndex = 0;

            Log.Info("ReadOriginalFormDb done");
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
