using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;
using System.Net.Mail;
using System.Transactions;

namespace CheckingIn
{
    public partial class CheckingIn : Form
    {
        /// <summary>
        /// 处理后的表格
        /// </summary>
        private DataTable _resultdt;
        /// <summary>
        /// 原始表格
        /// </summary>
        private DataTable _xlsdt;
        private DataTable _listdt;

        public DataTable OAdt;

        private DataTable _warnTable;

        private string _openedFleName;
        private readonly TimeSpan contInTime = new TimeSpan(0, 9, 30, 0);
        public static CheckingIn inst;
        /// <summary>
        /// 有人出勤的日期
        /// </summary>
        private DataTable alldates;
        private DataTable allnames;

        private SQLiteConnection _db;


        private TimeSpan delaytime;
        private int noworkdaycount;
        private int workdaycount;
        private TimeSpan allWorkTime;
        private const string Smtpusername = "oatool@yj543.com";
        private const string Smtppassword = "123qweASD";


        Dictionary<string, string> _classTime = new Dictionary<string, string>();
        Dictionary<string, string> _mail = new Dictionary<string, string>();
        public CheckingIn()
        {
            inst = this;

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            openFileDialog1.FileOk += OpenFileDialog1_FileOk;
            listView2.RetrieveVirtualItem += ListView2_RetrieveVirtualItem;

            InstTable();
            Checkdbfile();

            Readoa();
            ReadClassTime();
            Readmaildb();

        }

        private void Readoa()
        {
            var sql = "select * from oa order by no asc";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();

            OAdt.Load(reader);
        }

        internal void OaAdd(string name, DateTime s, DateTime e, string r)
        {
            var sql = $"insert into oa (name,start,end,reason) values ('{name}','{s.ToString("s")}','{e.ToString("s")}','{r}')";
            var ex = dbcmd(sql);
        }

        private void Opendb()
        {
            _db = new SQLiteConnection("Data Source=db.db");
            _db.Open();
        }

        private void Readmaildb()
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

        private void NewdbTable()
        {
            dbcmd("create table mail (name varchar(20), mail varchar(50))");
            dbcmd("create table classtime (name varchar(20), classname varchar(20))");
            dbcmd("create table result (name varchar(20), date date,intime time,outtime time,worktime time)");
            dbcmd("create table warn (name varchar(20), date date,txt varchar(20))");
            dbcmd("create table xls (name varchar(20), date date,time time)");
            dbcmd("create table oa (no integer primary key,name varchar(20), start datetime,end datatime,reason varchar(20))");
        }

        private void Checkdbfile()
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




        private int dbcmd(string cmd, SQLiteTransaction tran = null)
        {

            var command = new SQLiteCommand(cmd, _db);
            if (tran != null)
            {
                command.Transaction = tran;
            }

            return command.ExecuteNonQuery();
        }

        private void InstTable()
        {
            //结果表
            _resultdt = new DataTable();

            _resultdt.Columns.Add("name", typeof(string));
            _resultdt.Columns.Add("date", typeof(DateTime));
            _resultdt.Columns.Add("intime", typeof(TimeSpan));
            _resultdt.Columns.Add("outtime", typeof(TimeSpan));
            _resultdt.Columns.Add("worktime", typeof(TimeSpan));


            //警告表
            _warnTable = new DataTable();
            _warnTable.Columns.Add("name", typeof(string));
            _warnTable.Columns.Add("date", typeof(DateTime));
            _warnTable.Columns.Add("txt", typeof(string));


            //二次处理的表

            _xlsdt = new DataTable();

            _xlsdt.Columns.Add("name", typeof(string));
            _xlsdt.Columns.Add("date", typeof(DateTime));
            _xlsdt.Columns.Add("time", typeof(TimeSpan));


            //OA表
            OAdt = new DataTable();
            OAdt.Columns.Add("no", typeof(int));
            OAdt.Columns.Add("name", typeof(string));
            OAdt.Columns.Add("start", typeof(DateTime));
            OAdt.Columns.Add("end", typeof(DateTime));
            OAdt.Columns.Add("reason", typeof(string));

        }
        /// <summary>
        /// 原始表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                _listdt.Rows[e.ItemIndex]["name"].ToString(),
                ((DateTime)_listdt.Rows[e.ItemIndex]["date"]).ToShortDateString(),
                _listdt.Rows[e.ItemIndex]["time"].ToString(),
            });
        }

        private void OpenDataFile()
        {
            try
            {
                _openedFleName = openFileDialog1.FileName;
                var dt = ExcelToDs(openFileDialog1.FileName, "123");

                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    //读出时间
                    var time = DateTime.Parse(i["日期时间"].ToString());

                    //如果 时间是 05:00前的 就把日期算到前一天上面去
                    TimeSpan tt;
                    if (time.TimeOfDay < new TimeSpan(5, 0, 0))
                    {
                        time = time.AddDays(-1);
                        tt = time.TimeOfDay.Add(new TimeSpan(1, 0, 0, 0));//时间值多一天

                        Warn(i["姓名"].ToString(), time, "凌晨打卡");
                    }
                    else
                    {
                        tt = time.TimeOfDay;
                    }
                    //增加新记录
                    xlsadd(i["姓名"].ToString(), time.Date, tt);

                }
                //把OA数据加入进去
                foreach (DataRow i in OAdt.Rows)
                {
                    switch (i["reason"].ToString())
                    {
                        case "加班":
                        case "外出":
                            xlsadd(i["name"].ToString(), (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, i["reason"] + "start");
                            xlsadd(i["name"].ToString(), (DateTime)i["end"], ((DateTime)i["end"]).TimeOfDay, i["reason"] + "end");
                            break;
                        case "补登":
                            xlsadd(i["name"].ToString(), (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, i["reason"].ToString());
                            break;
                        case "出差":
                            //出差期间,每天自动增加一个上班打卡 和下班打卡
                            var s = (DateTime)i["start"];
                            var ee = (DateTime)i["end"];

                            //去掉时间
                            s = s.Date;
                            ee = ee.Date;
                            //得到出差几天
                            var days = ee - s;

                            for (int d = 0; d <= days.Days; d++)
                            {
                                xlsadd(i["name"].ToString(), s + new TimeSpan(d, 0, 0, 0), new TimeSpan(0, 9, 30, 0), i["reason"].ToString());
                                xlsadd(i["name"].ToString(), s + new TimeSpan(d, 0, 0, 0), new TimeSpan(0, 18, 30, 0));
                            }

                            break;
                    }


                }
                //得到所有有人出勤的日期
                var dv = new DataView(_xlsdt);
                alldates = dv.ToTable(true, "date");
                //读出所有姓名
                allnames = dv.ToTable(true, "name");


                var t = new Thread(Changedata);
                t.Start();
            }
            catch (Exception ex)
            {

                Console.Out.WriteLine(ex.ToString());
            }
        }

        private void OpenClasstimeFile()
        {
            var bt = _db.BeginTransaction();

            try
            {
                _openedFleName = openFileDialog1.FileName;
                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");


                //开始事务

                //把数据库里的清掉
                dbcmd("delete from classtime");

                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    //读出时间
                    var name = i["姓名"].ToString();
                    //读出时间
                    var classname = i["对应时段"].ToString();
                    //写到表里

                    dbcmd($"insert into classtime (name,classname) values ('{name}','{classname}')");


                }
                bt.Commit();

                ReadClassTime();

            }
            catch (Exception ex)
            {
                bt.Rollback();
                throw ex;
            }



        }
        private void OpenMailFile()
        {
            _openedFleName = openFileDialog1.FileName;

            var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");
            //开始事务

            //把数据库里的清掉
            dbcmd("delete from mail");

            //进行遍历处理 生成新的表
            foreach (DataRow i in dt.Tables[0].Rows)
            {
                //读出时间
                var name = i["姓名"].ToString();
                //读出时间
                var mail = i["邮箱"].ToString();
                //写到表里

                dbcmd($"insert into mail (name,mail) values ('{name}','{mail}')");
            }
            Readmaildb();

        }
        private void ReadClassTime()
        {
            //从表中得到数据

            var sql = "select * from classtime";
            var command = new SQLiteCommand(sql, _db);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                _classTime.Add(reader["name"].ToString(), reader["classname"].ToString());
            }

        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            if (openFileDialog1.Title == "选择考勤器原始文件")
            {
                OpenDataFile();

            }
            else if (openFileDialog1.Title == "选择班次文件")
            {
                OpenClasstimeFile();

            }
            else if (openFileDialog1.Title == "选择邮箱文件")
            {
                OpenMailFile();

            }




        }
        /// <summary>
        /// 向处理后的;xls表增加数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="t"></param>
        /// <param name="rs"></param>
        public void xlsadd(string name, DateTime date, TimeSpan t, string rs = null)
        {
            var r = _xlsdt.NewRow();
            r["name"] = name;
            r["date"] = date.Date;
            r["time"] = t;
            _xlsdt.Rows.Add(r);

            if (rs != null)
            {
                Warn(name, date, rs);
            }

        }

        public DataSet ExcelToDs(string path, string tablename)
        {
            var strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=Excel 8.0;";
            var conn = new OleDbConnection(strConn);
            conn.Open();
            var strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = $"select * from [{tablename}$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds);
            return ds;
        }
        /// <summary>
        /// 异常警告增加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        /// <param name="t"></param>
        private void Warn(string name, object dt, string t)
        {
            var wr = _warnTable.NewRow();
            wr["name"] = name;
            wr["date"] = ((DateTime)dt).Date;
            wr["txt"] = t;
            _warnTable.Rows.Add(wr);
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        private void Changedata()
        {




            toolStripProgressBar1.Maximum = allnames.Rows.Count;
            toolStripProgressBar1.Value = 0;

            //对每人个进行遍历
            foreach (DataRow r in allnames.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();
                comboBox1.Items.Add(n);


                toolStripProgressBar1.Value += 1;
            }
            comboBox1.SelectedIndex = 0;

        }

        private void NewResultRecord(string name, object dt, object intime, object outtime, out TimeSpan worktime)
        {
            var rr = _resultdt.NewRow();
            rr["name"] = name;
            rr["date"] = dt;
            rr["intime"] = intime;
            rr["outtime"] = outtime;
            var wt = (TimeSpan)outtime - (TimeSpan)intime;
            rr["worktime"] = wt;
            worktime = wt;
            _resultdt.Rows.Add(rr);

        }
        private TimeSpan GetOverWorkTimeCount(string name)
        {
            var t = new TimeSpan();
            var dv = new DataView(OAdt) { RowFilter = $"name = '{name}' and reason ='加班'" };
            foreach (DataRowView dr in dv)
            {
                var s = (DateTime)dr.Row["start"];
                var ee = (DateTime)dr.Row["end"];
                t += ee - s;
            }
            return t;
        }

        private int GetOutDaysCount(string name)
        {
            var t = new TimeSpan();
            var dv = new DataView(OAdt) { RowFilter = $"name = '{name}' and reason ='出差'" };
            foreach (DataRowView dr in dv)
            {
                var s = (DateTime)dr.Row["start"];
                var ee = (DateTime)dr.Row["end"];
                t += ee - s;
            }
            return t.Days + 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            //对当前数据进行处理
            GetData(comboBox1.Text);

            //统计信息




            label5.Text = $"{workdaycount - noworkdaycount}/{workdaycount}\r\n{allWorkTime.TotalHours.ToString(".##")}/{workdaycount * 8}\r\n{delaytime.TotalMinutes.ToString("###0")}\r\n{GetOutDaysCount(comboBox1.Text)}\r\n{GetOverWorkTimeCount(comboBox1.Text).TotalHours.ToString(".##")}";




            //在日期控件上加粗显示有数据的日期
            var dv = new DataView(_resultdt) { RowFilter = $"name = '{comboBox1.Text}'" };

            monthCalendar1.RemoveAllBoldedDates();
            foreach (DataRowView dr in dv)
            {
                monthCalendar1.AddBoldedDate((DateTime)dr.Row["date"]);
            }
            var v = monthCalendar1.BoldedDates[0];

            monthCalendar1.SetDate(v.AddMonths(1));
            monthCalendar1.SetDate(v);




            //得到相关警告记录
            dv = new DataView(_warnTable) { RowFilter = $"name = '{comboBox1.Text}'", Sort = "date asc" };


            listView_warn.Items.Clear();
            foreach (DataRowView i in dv)
            {
                listView_warn.Items.Add(
                    new ListViewItem(new[]
                    {
                        ((DateTime) i.Row["date"]).ToShortDateString(),
                        i.Row["txt"].ToString()   })
                    );
            }


        }

        private void listView_warn_SelectedIndexChanged(object sender, EventArgs e)
        {
            //得到日期
            if (listView_warn.SelectedItems.Count == 0)
                return;

            var d = listView_warn.SelectedItems[0].SubItems[0].Text;

            var dt = DateTime.Parse(d);

            //选定日期
            monthCalendar1.SetDate(dt);

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            //防空
            if (_xlsdt.Rows.Count == 0) return;
            //得到这一天的警告数据
            var dv = new DataView(_warnTable) { RowFilter = $"name = '{comboBox1.Text}' AND date = '{e.Start}'" };
            var warntxt = "";
            foreach (DataRowView i in dv)
            {
                warntxt += i.Row["txt"] + " ";
            }
            if (warntxt == "")
                warntxt = "正常";

            //得到这个用户 当天的记录
            dv = new DataView(_resultdt) { RowFilter = $"name = '{comboBox1.Text}' AND date = '{e.Start}'" };
            //结果记录 一天就应该是一条
            if (dv.Count > 0)
            {
                label4.Text = "";
                label4.Text += ((DateTime)dv[0]["date"]).ToShortDateString() + "\r\n";
                label4.Text += dv[0]["intime"] + "\r\n";
                label4.Text += dv[0]["outtime"] + "\r\n";
                label4.Text += dv[0]["worktime"] + "\r\n";
                label4.Text += warntxt;

            }
            else
            {
                label4.Text = monthCalendar1.SelectionStart.ToShortDateString() + "\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n" + warntxt;
            }

            //原来的记录
            dv = new DataView(_xlsdt) { RowFilter = $"name = '{comboBox1.Text}' AND date = '{e.Start}'" };
            _listdt = dv.ToTable();
            listView2.VirtualListSize = _listdt.Rows.Count;
            listView2.Invalidate();



        }
        public void WriteExcel(DataTable dt, string path)
        {
            Thread.Sleep(1000);


            var sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));//打开写文件流
            var sb = new StringBuilder();



            //写标题
            for (var k = 0; k < dt.Columns.Count; k++)
            {
                sb.Append(dt.Columns[k].ColumnName + "\t");
            }
            sb.Append(Environment.NewLine);

            //写内容

            for (var i = 0; i < dt.Rows.Count; i++)//遍历每行内容
            {
                Application.DoEvents();

                for (var j = 0; j < dt.Columns.Count; j++)//一行中的每列
                {
                    sb.Append(dt.Rows[i][j] + "\t");//每个单元格内容，加到StringBuilder中
                }
                sb.Append(Environment.NewLine);
            }

            sw.Write(sb.ToString());//文件流写入内容
            sw.Flush();
            sw.Close();


        }

        private void GetData(string name)
        {

            //防止重复处理
            var ex = _resultdt.Select($"name ='{name}'");
            if (ex.Length > 0) return;



            toolStripProgressBar1.Maximum = alldates.Rows.Count;
            toolStripProgressBar1.Value = 0;


            delaytime = new TimeSpan();
            noworkdaycount = 0;
            workdaycount = 0;
            allWorkTime = new TimeSpan();
            var workclass = "早班";
            try
            {
                workclass = _classTime[name];
            }
            catch (Exception )
            {
               
                
            }
           

            //对所有有人出勤的日期进行遍历
            foreach (DataRow dater in alldates.Rows)
            {
                toolStripProgressBar1.Value += 1;
                //今日日期
                var date = dater["date"];

                var isworkday = false;//对于综合班次的人来说所有都是非工作日


                //判断是不是工作日
                //如果有30个出勤,就算工作日
                var dateview = new DataView(_xlsdt) { RowFilter = $"date ='{date}'" };
                isworkday = dateview.Count > 50;
                if (isworkday) workdaycount++;



                //得到这个人今天所有的打卡时间
                var timeview = new DataView(_xlsdt)
                {
                    RowFilter = $"name = '{name}' AND date = '{date}'",
                    Sort = "time asc" //从小到大
                };
                //无打卡记录
                if (timeview.Count == 0)
                {
                    if (workclass != "综合班次" && isworkday)
                    {
                        noworkdaycount++;
                        Warn(name, date, "旷工");
                    }


                    continue;//当天没有记录 返回
                }

                //进行记录
                var intime = (TimeSpan)timeview[0].Row["time"];
                var outtime = (TimeSpan)timeview[timeview.Count - 1].Row["time"];
                TimeSpan wt;
                NewResultRecord(name, date, intime, outtime, out wt);


                //相关警告
                if (timeview.Count < 2)
                {
                    Warn(name, date, "少打卡");//综合工时也是要有一天两次卡才可以
                }
                else
                {
                    //两次打卡完整 

                    TimeSpan classInTime;
                    TimeSpan classOutTime;
                    string classname;

                    getClassTime(name, out classInTime, out classOutTime, out classname);

                    if (classname == "综合班次")
                    {
                        //累计工作时间
                        allWorkTime += wt;

                    }
                    else
                    {
                        if (intime > classInTime.Add(new TimeSpan(0, 0, 30, 0)))//30分钟缓冲时间
                        {
                            var d = intime - contInTime;
                            delaytime += d;
                            Warn(name, date, "迟到");
                        }

                        if (outtime < classInTime.Add(new TimeSpan(0, 9, 0, 0)))
                        {
                            Warn(name, date, "早退");
                        }

                    }

                    if (workclass != "综合班次" && !isworkday)
                        Warn(name, date, "疑似加班");


                }
            }
        }

        private void getClassTime(string name, out TimeSpan classInTime, out TimeSpan classOutTime, out string cn)
        {

            cn = _classTime[name];

            classInTime = new TimeSpan();
            classOutTime = new TimeSpan();

            switch (cn)
            {
                case "早班":
                    classInTime = new TimeSpan(0, 9, 0, 0);
                    classOutTime = new TimeSpan(0, 18, 0, 0);
                    break;
                case "中班":
                    classInTime = new TimeSpan(0, 9, 30, 0);
                    classOutTime = new TimeSpan(0, 18, 30, 0);
                    break;
                case "晚班":
                    classInTime = new TimeSpan(0, 11, 30, 0);
                    classOutTime = new TimeSpan(0, 20, 30, 0);
                    break;
                case "特别班次":
                    classInTime = new TimeSpan(0, 12, 0, 0);
                    classOutTime = new TimeSpan(0, 21, 0, 0);
                    break;
                case "综合班次":
                    classInTime = new TimeSpan(0, 0, 0, 0);
                    classOutTime = new TimeSpan(0, 0, 0, 0);
                    break;
            }

        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "考勤器原始文件|*.xls";
            openFileDialog1.Title = "选择考勤器原始文件";
            openFileDialog1.ShowDialog();
        }


        private void 增加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new oadata();
            f.Show();
        }

        private void CheckingIn_FormClosed(object sender, FormClosedEventArgs e)
        {
            _db.Close();
        }

        private void 输出文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //从结果进行遍历
            if (alldates.Rows.Count == 0)
                return;

            var count = 0;
            var dt = new DataTable();
            dt.Columns.Add("name");
            foreach (DataRow d in alldates.Rows)
            {
                dt.Columns.Add(((DateTime)d["date"]).ToShortDateString());
            }

            //对每人个进行遍历
            foreach (DataRow n in allnames.Rows)
            {
                //当前人名字
                var name = n["name"].ToString();
                comboBox1.Items.Add(n);


                var dr = dt.NewRow();
                dr["name"] = name;

                GetData(name);

                foreach (DataRow d in alldates.Rows)
                {
                    //得到这一天的警告数据
                    var dv = new DataView(_warnTable) { RowFilter = $"name = '{name}' AND date = '{d["date"]}'" };
                    var warntxt = "";
                    foreach (DataRowView i in dv)
                    {
                        warntxt += i.Row["txt"] + " ";
                    }
                    if (warntxt == "")
                        warntxt = "正常";

                    dr[((DateTime)d["date"]).ToShortDateString()] = warntxt;
                }

                dt.Rows.Add(dr);





                count++;
                toolStripProgressBar1.Maximum = allnames.Rows.Count;
                toolStripProgressBar1.Value = count;
            }

            WriteExcel(dt, "test.xls");

        }

        private void 读取班次表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "班次文件|*.xls";
            openFileDialog1.Title = "选择班次文件";
            openFileDialog1.ShowDialog();
        }

        private void 读取邮箱表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "邮箱文件|*.xls";
            openFileDialog1.Title = "选择邮箱文件";
            openFileDialog1.ShowDialog();
        }


        private void sendmail(string name, string mail)
        {

            if (mail.IndexOf("@") == -1)
            {
                return;
            }

            //合成文字
            try
            {
                GetData(name);

                //统计信息
                var body = $"{name}-考勤分析报表\r\n";

                body += $"实到/应到:{workdaycount - noworkdaycount}/{workdaycount}\r\n" +
                          $"综合工作/应到工时:{allWorkTime.TotalHours.ToString(".##")}/{workdaycount * 8}\r\n" +
                          $"迟到(分):{delaytime.TotalMinutes.ToString("####")}\r\n" +
                          $"出差(天):{GetOutDaysCount(comboBox1.Text).ToString("00")}\r\n" +
                          $"加班:{GetOverWorkTimeCount(comboBox1.Text).TotalHours.ToString("00")}";

                body += "\r\n\r\n所有警告信息\r\n";
                //得到所有警告信息
                var dv = new DataView(_warnTable) { RowFilter = $"name = '{name}'", Sort = "date asc" };

                foreach (DataRowView i in dv)
                {
                    body += $"{((DateTime)i.Row["date"]).ToShortDateString()}-{i.Row["txt"]}\r\n";
                }

                //发出去

                var smtp = new SmtpClient()
                {
                    Host = "smtp.exmail.qq.com",

                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Smtpusername, Smtppassword),
                    EnableSsl = true,
                    // Port = 465


                };


                smtp.Send(Smtpusername, mail, $"{name}-考勤分析报表", body);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void 向全体发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var k = 0;
            //对每人个进行遍历
            foreach (DataRow r in allnames.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();


                sendmail(n, _mail[n]);



                k++;
                toolStripProgressBar1.Maximum = allnames.Rows.Count;
                toolStripProgressBar1.Value = k;


            }

        }

        private void 向当前用户发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sendmail(comboBox1.Text, _mail[comboBox1.Text]);
        }
    }
}
