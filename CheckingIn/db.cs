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

        private static void Opendb()
        {
            _db = new SQLiteConnection("Data Source=db.db");
            _db.Open();
        }
        
        private static void CreatSqlTable()
        {
            Cmd("create table person (name varchar(20) primary key , mail varchar(50),worktimeclass varchar(20),password varchar(20))");
            Cmd("create table original (name varchar(20), date datetime,time INTEGER,info varchar(20))");
            Cmd("create table oa (no integer primary key,name varchar(20),date date, start datetime,end datetime,reason varchar(20),reason varchar(20))");


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

        public static void Creat()
        {
            CheckSqlFile();
        }
        
        public static void Close()
        {
            _db.Close();
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
    }
}
