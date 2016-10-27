using System;
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
using System.Reflection;
using System.Resources;

namespace CheckingIn
{
    public partial class CheckingIn : Form
    {


        private string _openedFleName;
        public static CheckingIn inst;


        private const string Smtpusername = "oatool@yj543.com";
        private const string Smtppassword = "123qweASD";



        /// <summary>
        /// 全公司的人
        /// </summary>
        Dictionary<string, PersonInfo> persons = new Dictionary<string, PersonInfo>();


        private DataTable _alldates, _allnames;


        public CheckingIn()
        {
            inst = this;

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            openFileDialog1.FileOk += OpenFileDialog1_FileOk;
            listView2.RetrieveVirtualItem += ListView2_RetrieveVirtualItem;

            DB.Creat();



            Log.Creat(listView_log);

        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            switch (openFileDialog1.Title)
            {
                case "选择考勤器原始文件":
                    var t = new Thread(OpenDataFile);
                    t.Start();
                    break;
                case "选择班次文件":
                    OpenWorkTimeClassFile();
                    break;
                case "选择邮箱文件":
                    OpenMailFile();
                    break;
                case "选择加班文件":
                    OpenOverworkFile();
                    break;
                case "选择外出文件":
                    OpenOutFile();
                    break;
                case "选择出差文件":
                    OpenOutworkFile();
                    break;
                case "选择补登文件":
                    OpenAddCheckinFile();
                    break;
            }
        }

        private void OpenDataFile()
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

                    DB.AddWarn(i["姓名"].ToString(), time, "凌晨打卡");
                }
                else
                {
                    tt = time.TimeOfDay;
                }
                //增加新记录
                DB.AddOriginal(i["姓名"].ToString(), time.Date, tt);

            }


            //把OA数据加入进去
            foreach (DataRow i in DB.OaDt.Rows)
            {
                switch (i["reason"].ToString())
                {
                    case "加班":
                    case "外出":
                        DB.AddOriginal(i["name"].ToString(), (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, i["reason"] + "start");
                        DB.AddOriginal(i["name"].ToString(), (DateTime)i["end"], ((DateTime)i["end"]).TimeOfDay, i["reason"] + "end");
                        break;
                    case "补登":

                        DB.AddOriginal(i["name"].ToString(), (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, i["reason"].ToString());

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
                            DB.AddOriginal(i["name"].ToString(), s + new TimeSpan(d, 0, 0, 0), new TimeSpan(0, 9, 30, 0), i["reason"].ToString());
                            DB.AddOriginal(i["name"].ToString(), s + new TimeSpan(d, 0, 0, 0), new TimeSpan(0, 18, 30, 0));
                        }

                        break;
                    case "请假":

                        break;
                }
            }

            //得到所有有人出勤的日期
            var dv = new DataView(DB.OriginalDt);
            //读出所有姓名
            _allnames = dv.ToTable(true, "name");

            //得到所有人出勤的日子
            _alldates = dv.ToTable(true, "Date");

            //对每人个进行遍历
            foreach (DataRow r in _allnames.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();
                comboBox1.Items.Add(n);
                persons.Add(n, new PersonInfo(n));
            }


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

            comboBox1.SelectedIndex = 0;


        }

        private void OpenWorkTimeClassFile()
        {
            var bt = DB.BeginTransaction();

            try
            {
                _openedFleName = openFileDialog1.FileName;
                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");

                //开始事务

                //进行遍历处理    
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    //读出时间
                    var name = i["姓名"].ToString();
                    //读出时间
                    var classname = i["对应时段"].ToString();
                    //写到表里
                    var dv = new DataView(DB.Persons) { RowFilter = $"name='{name}'" };
                    DB.Cmd(dv.Count > 0
                        ? $"update person set worktimeclass='{classname}' where name='{name}'"
                        : $"insert into person (name,worktimeclass) values ('{name}','{classname}')", bt);
                }
                bt.Commit();
                DB.Readpersondb();
                Log.Info("worktimeclass done");
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

            var bt = DB.BeginTransaction();

            try
            {
                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    //读出时间
                    var name = i["姓名"].ToString();
                    //读出时间
                    var mail = i["邮箱"].ToString();
                    //写到表里
                    var dv = new DataView(DB.Persons) { RowFilter = $"name='{name}'" };
                    DB.Cmd(dv.Count > 0
                        ? $"update person set mail='{mail}' where name='{name}'"
                        : $"insert into person (name,mail) values ('{name}','{mail}')", bt);
                }
                bt.Commit();

                DB.Readpersondb();
                Log.Info("mail done");
            }
            catch (Exception ex)
            {
                bt.Rollback();

                throw ex;
            }
        }


        private void OpenOverworkFile()
        {
            var bt = DB.BeginTransaction();
            try
            {
                _openedFleName = openFileDialog1.FileName;

                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");
                //开始事务


                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["加班开始"].ToString());
                    var se = DateTime.Parse(i["加班结束"].ToString());
                    var d = i["天"].ToString();
                    var h = i["小时"].ToString();

                    //写到表里

                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaAdd(name, st, se, "加班");

                }
                bt.Commit();
            }
            catch (Exception ex)
            {
                Log.Warn("加班写入数据库出现问题" + ex.Message);
                bt.Rollback();
                throw;
            }


        }
        private void OpenOutFile()
        {
            var bt = DB.BeginTransaction();
            try
            {
                _openedFleName = openFileDialog1.FileName;

                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");
                //开始事务


                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["外出时间"].ToString());
                    var se = DateTime.Parse(i["实际返回"].ToString());

                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaAdd(name, st, se, "外出");

                }
                bt.Commit();
            }
            catch (Exception ex)
            {
                Log.Warn("外出写入数据库出现问题" + ex.Message);
                bt.Rollback();
                throw;
            }


        }

        private void OpenAddCheckinFile()
        {//开始事务
            var bt = DB.BeginTransaction();
            try
            {
                _openedFleName = openFileDialog1.FileName;

                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");


                var count = new Dictionary<string, int>();

                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {


                    var name = i["姓名"].ToString();

                    if (count.ContainsKey(name))
                    {
                        count[name] = count[name] + 1;
                        if (count[name] >= 4)
                        {
                            Log.Err(name + "-超出3次补登,请注意");
                            continue;
                        }
                    }
                    else
                    {
                        count.Add(name, 1);
                    }

                    var st = DateTime.Parse(i["打卡异常时间"].ToString());
                    var r = i["上班或下班打卡异常"].ToString();
                    if (st.Month != monthCalendar1.SelectionStart.Month)//月份不对
                        continue;
                    string c;
                    TimeSpan s, e;
                    /* todo
                    GetClassTime(Name, out s, out e, out c);

                    if (r == "上班")
                    {
                        st = st.Add(s);
                    }
                    else
                    {
                        st = st.Add(e);
                    }*/

                    //写到表里

                    DB.OaAdd(name, st, st, "补登");
                }

                bt.Commit();
            }
            catch (Exception ex)
            {
                Log.Warn("补登写入数据库出现问题" + ex.Message);
                bt.Rollback();
                throw;
            }


        }
        private void OpenOutworkFile()
        {//开始事务
            var bt = DB.BeginTransaction();
            try
            {
                _openedFleName = openFileDialog1.FileName;

                var dt = ExcelToDs(openFileDialog1.FileName, "Sheet1");



                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["实际开始"].ToString());
                    var se = DateTime.Parse(i["实际结束"].ToString());

                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaAdd(name, st, se, "出差");
                }

                bt.Commit();
            }
            catch (Exception ex)
            {
                Log.Warn("出差写入数据库出现问题" + ex.Message);
                bt.Rollback();
                throw;
            }


        }




        public DataSet ExcelToDs(string path, string tablename)
        {
            //todo 修正表名的问题
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




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {



            //对当前数据进行处理

            var p = persons[comboBox1.Text];

            p.GetData();

            //统计信息

            label5.Text = $"{WorkDay.WorkCount - p.WarnDayCount}/{WorkDay.WorkCount}\r\n{p.WorkTime.TotalHours.ToString(".#")}/{WorkDay.WorkCount * 8}\r\n{p.DelayTime.TotalMinutes.ToString("####")}\r\n{p.Travel}\r\n{p.WorkTime.TotalHours.ToString(".##")}";

            monthCalendar1.RemoveAllBoldedDates();
            listView_warn.Items.Clear();

            foreach (var c in p.Checks)
            {

                foreach (var w in c.Warns)
                {
                    monthCalendar1.AddBoldedDate(w.Date);
                    listView_warn.Items.Add(
                   new ListViewItem(new[]
                   {
                       w.Date.ToString(),
                        w.Info  })
                   );
                }
            }

            var v = monthCalendar1.BoldedDates.Length > 0 ? monthCalendar1.BoldedDates[0] : monthCalendar1.SelectionStart;

            monthCalendar1.SetDate(v.AddMonths(1));
            monthCalendar1.SetDate(v);


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
            if(comboBox1.Text=="")return;

            var p = persons[comboBox1.Text];
            var check = p.GetCheck(e.Start);

            if (check != null)
            {
                var warntxt = "";
                foreach (var i in check.Warns)
                {
                    warntxt += i.Info + " ";
                }
                if (warntxt == "")
                    warntxt = "正常";



                label4.Text = "";
                label4.Text += check.Date + "\r\n";
                label4.Text += check.InTime + "\r\n";
                label4.Text += check.OutTime + "\r\n";
                label4.Text += check.WorkTime + "\r\n";
                label4.Text += warntxt;



                //原来的记录

                DB.OriginalListDt = check.Sourcerec.ToTable();
                listView2.VirtualListSize = DB.OriginalListDt.Rows.Count;
                listView2.Invalidate();


            }
            else
            {
                label4.Text = monthCalendar1.SelectionStart.ToShortDateString() + "\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\nnodata";
            }
        }





        /// <summary>
        /// 原始表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                DB.OriginalListDt.Rows[e.ItemIndex]["name"].ToString(),
                ((DateTime)DB.OriginalListDt.Rows[e.ItemIndex]["Date"]).ToShortDateString(),
                DB.OriginalListDt.Rows[e.ItemIndex]["time"].ToString(),
            });
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            DB.Close();
        }

        private void 输出文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //从结果进行遍历
            if (_alldates.Rows.Count == 0)
                return;

            var count = 0;
            var dt = new DataTable();
            dt.Columns.Add("name");
            foreach (DataRow d in _alldates.Rows)
            {
                dt.Columns.Add(((DateTime)d["Date"]).Day.ToString());
            }

            //对每人个进行遍历
            foreach (DataRow n in _allnames.Rows)
            {
                //当前人名字
                var name = n["name"].ToString();

                var dr = dt.NewRow();
                dr["name"] = name;

                //GetData(Name);
                //todo

                foreach (DataRow d in _alldates.Rows)
                {
                    //得到这一天的警告数据
                    var dv = new DataView(DB.WarnDt) { RowFilter = $"name = '{name}' AND date = '{d["Date"]}'" };
                    var warntxt = "";
                    foreach (DataRowView i in dv)
                    {
                        warntxt += i.Row["txt"] + " ";
                    }
                    if (warntxt == "")
                        warntxt = "正常";

                    dr[((DateTime)d["Date"]).Day.ToString()] = warntxt;
                }

                dt.Rows.Add(dr);

                count++;
                toolStripProgressBar1.Maximum = _allnames.Rows.Count;
                toolStripProgressBar1.Value = count;
            }

            WriteExcel(dt, "test.xls");

        }

        private void 读取班次表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择班次文件";
            openFileDialog1.ShowDialog();
        }

        private void 读取邮箱表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择邮箱文件";
            openFileDialog1.ShowDialog();
        }

        private string GetHtmltr(string a, string b)
        {
            return $"<tr><td class=\"text-left\">{a}</td><td class=\"text-left\">{b}</td></tr>";
        }

        private string GetHtml(string b)
        {
            var r = new ResourceManager("htmlhead", Assembly.GetExecutingAssembly());




            var e = "</body>";


            return b + e;
        }

        private void Sendmail(PersonInfo p)
        {


            //合成文字
            try
            {
                if (p.Mail.IndexOf("@") == -1)
                {
                    throw new Exception("邮箱地址不合法");
                }

                p.GetData();

                //统计信息
                var body = $"{p.Name}-考勤分析报表\r\n";


                if (p.WorkTimeClass.IsWorkTimeClass)
                    body += GetHtmltr("工时/应到工时", $"{p.WorkTime.TotalHours.ToString(".#")}小时/{WorkDay.WorkCount * 8}小时");
                else
                    body += GetHtmltr("实到/应到", $"{p.WarnDayCount}天/{WorkDay.WorkCount}天");


                body += GetHtmltr("迟到", p.DelayTime.TotalMinutes.ToString("## '分钟'"));
                body += GetHtmltr("出差", p.Travel.TotalDays.ToString("00'天'"));
                body += GetHtmltr("加班", p.OverWorkTime.TotalHours.ToString("00'小时'"));


                body += "<h2>(请假暂时没有接入系统)</h2>";


                //得到所有警告信息
                var dv = new DataView(DB.WarnDt) { RowFilter = $"name = '{p.Name}'", Sort = "Date asc" };

                foreach (DataRowView i in dv)
                {
                    body += $"{((DateTime)i.Row["Date"]).ToShortDateString()}-{i.Row["txt"]}\r\n";
                }
                //所有结果数据
                body += "\r\n\r\n所有记录信息\r\n";
                body += "日期\t上班时间\t下班时间\r\n";
                dv = new DataView(DB.Resultdt) { RowFilter = $"name = '{p.Name}'" };
                foreach (DataRowView i in dv)
                {
                    /*
                           <tr><td class="text-left">出差</td><td class="text-left">99天</td></tr>
                     */

                    body += $"{((DateTime)i.Row["Date"]).ToShortDateString()} \t {i.Row["intime"]} \t {i.Row["outtime"]}\r\n";
                }


                body = GetHtml(body);
                //发出去

                var smtp = new SmtpClient()
                {
                    Host = "smtp.exmail.qq.com",

                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Smtpusername, Smtppassword),
                    EnableSsl = true,
                };


                smtp.Send(Smtpusername, p.Mail, $"{p.Name}-考勤分析报表", body);
            }
            catch (Exception ex)
            {

                Log.Err("发送邮件-" + ex.Message);
            }

        }

        private void 向全体发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var k = 0;
            //对每人个进行遍历
            foreach (var p in persons)
            {
                Sendmail(p.Value);
                k++;
                toolStripProgressBar1.Maximum = _allnames.Rows.Count;
                toolStripProgressBar1.Value = k;
            }

        }

        private void 向当前用户发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sendmail(persons[comboBox1.Text]);
        }

        private void 读取加班文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择加班文件";
            openFileDialog1.ShowDialog();
        }

        private void 读取外出文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择外出文件";
            openFileDialog1.ShowDialog();
        }


        private void 读取出差文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择出差文件";
            openFileDialog1.ShowDialog();
        }

        private void 读取补登文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "选择补登文件";
            openFileDialog1.ShowDialog();
        }

        private void 班次ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from worktimeclass";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }

        private void 加班ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from oa where reason ='加班'";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }

        private void 出差ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from oa where reason ='出差'";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }

        private void 外出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from oa where reason ='外出'";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }

        private void 补登ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from oa where reason ='补登'";


            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }

        private void 邮箱ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var sql = "select * from mail";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }



        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (comboBox1.AutoCompleteMode != AutoCompleteMode.SuggestAppend)
                comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void 清空OA数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DB.Cmd("delete from oa");
        }

        private void 个人信息表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.Persons);
            s.Show();
        }

        private void 结果表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.Resultdt);
            s.Show();
        }

        private void 原始表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.OriginalDt);
            s.Show();
        }

        private void oa表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.OaDt);
            s.Show();
        }

        private void 警告表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.WarnDt);
            s.Show();
        }

        private void 删除文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DB.Close();
            File.Delete("db.db");
            DB.Creat();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dateview = new DataView(DB.OriginalDt) { RowFilter = $"date ='{monthCalendar1.SelectionStart.Date}'" };//去重
            var pcount = dateview.ToTable(true, "name");
            var s = new ShowData(pcount);
            s.Show();
        }
    }

}
