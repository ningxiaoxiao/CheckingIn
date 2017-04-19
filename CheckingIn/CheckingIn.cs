using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using DotNet4.Utilities;
using LitJson2;

namespace CheckingIn
{
    public partial class CheckingIn : Form
    {

        public static CheckingIn Inst;


        private const string Smtpusername = "oatool@yj543.com";
        private const string Smtppassword = "123qweASD";

        private const string outputfilename = "out.xls";



        // private DataTable _alldates, _allnames;
        private readonly HttpSever _http;

        public JsonData workdaysjson { get; private set; }
        private const string Jsonyear = "2017";
        public CheckingIn()
        {
            Inst = this;

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            openFileDialog1.FileOk += OpenFileDialog1_FileOk;

            Log.Creat(listView_log);

           

            //得到非工作日
            _http = new HttpSever();

            var h = new HttpHelper();
            var i = new HttpItem()
            {
                URL = $"http://tool.bitefu.net/jiari/vip.php?d={Jsonyear}&type=0&apikey=123456",
                IsToLower = false,
            };


            var htmlstr = h.GetHtml(i).Html;
            workdaysjson = JsonMapper.ToObject(htmlstr);
            workdaysjson = workdaysjson["data"][Jsonyear];

            DB.Creat();





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
                    OpenWorkTimeClassFile(openFileDialog1.FileName);
                    break;
                case "选择邮箱文件":
                    OpenMailFile(openFileDialog1.FileName);
                    break;

            }

        }

        private void OpenDataFile()
        {


            var dt = new ExcelHelper(openFileDialog1.FileName).ExcelToDataTable("", true);

            DB.DelOrigina();
            try
            {
                DB.BeginTransaction();
                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    //读出时间
                    var time = DateTime.Parse(i["日期时间"].ToString());

                    //如果 时间是 05:00前的 就把日期算到前一天上面去
                    TimeSpan tt;
                    if (time.TimeOfDay < new TimeSpan(5, 0, 0))
                    {
                        time = time.AddDays(-1);
                        tt = time.TimeOfDay.Add(new TimeSpan(1, 0, 0, 0));//时间值多一天
                    }
                    else
                    {
                        tt = time.TimeOfDay;
                    }
                    //增加新记录
                    DB.AddCheckOriginal(i["姓名"].ToString(), time.Date, tt, "");
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                Log.Err("读取考勤器文件出错." + ex.Message);
            }

            DB.ReadOriginalFormDb();

            Log.Info("读取考勤器文件完成");




        }
        /// <summary>
        /// 列表中加人名
        /// </summary>
        /// <param name="t"></param>
        public static void comadd(string t)
        {
            Inst.comboBox1.Items.Add(t);
        }
        /// <summary>
        /// 班次表
        /// </summary>
        /// <param name="path"></param>
        private void OpenWorkTimeClassFile(string path)
        {
            DB.BeginTransaction();

            try
            {
                var dt = new ExcelHelper(path).ExcelToDataTable("", true);

                //开始事务

                //进行遍历处理    
                foreach (DataRow i in dt.Rows)
                {
                    //读出时间
                    var name = i["姓名"].ToString();
                    //读出时间
                    var classname = i["对应时段"].ToString();
                    //写到表里
                    var dv = new DataView(DB.PersonInfos) { RowFilter = $"name='{name}'" };
                    DB.Cmd(dv.Count > 0
                        ? $"update person set worktimeclass='{classname}' where name='{name}'"
                        : $"insert into person (name,worktimeclass) values ('{name}','{classname}')");
                }
                DB.Commit();
                DB.ReadPersonDb();
                Log.Info("worktimeclass done");
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
        }
        /// <summary>
        /// 邮件表
        /// </summary>
        /// <param name="path"></param>
        private void OpenMailFile(string path)
        {
            var dt = new ExcelHelper(path).ExcelToDataTable("", true);

            DB.BeginTransaction();

            try
            {
                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    //读出时间
                    var name = i["姓名"].ToString();
                    //读出时间
                    var mail = i["邮箱"].ToString();
                    //写到表里
                    var dv = new DataView(DB.PersonInfos) { RowFilter = $"name='{name}'" };
                    DB.Cmd(dv.Count > 0
                        ? $"update person set mail='{mail}' where name='{name}'"
                        : $"insert into person (name,mail) values ('{name}','{mail}')");
                }
                DB.Commit();

                DB.ReadPersonDb();
                Log.Info("mail done");
            }
            catch (Exception ex)
            {
                DB.Rollback();

                throw ex;
            }
        }

        #region 读取OA文件
        /// <summary>
        /// 加班
        /// </summary>
        /// <param name="path"></param>
        private void OpenOverworkFile(string path)
        {

            try
            {
                var dt = new ExcelHelper(openFileDialog1.FileName).ExcelToDataTable("", true, true);
                //开始事务
                //进行遍历处理 生成新的表
                DB.BeginTransaction();
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["加班开始"].ToString());
                    var se = DateTime.Parse(i["加班结束"].ToString());
                    var d = i["天"].ToString();
                    var h = i["小时"].ToString();

                    if (se < st)
                    {
                        Log.Err(name + "-非法单据,加班线束小于加班开始");
                        continue;
                    }

                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaOriginaAdd(name, st, se, "加班");

                }
                DB.Commit();

                Log.Info(path + "-read done");
            }
            catch (Exception ex)
            {
                DB.Rollback();
                Log.Err("加班写入数据库出现问题" + ex.Message);

                throw;
            }


        }
        /// <summary>
        /// 外出
        /// </summary>
        /// <param name="path"></param>
        private void OpenOutFile(string path)
        {
            DB.BeginTransaction();
            try
            {
                var dt = new ExcelHelper(openFileDialog1.FileName).ExcelToDataTable("", true, true);
                //开始事务


                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["外出时间"].ToString());
                    var se = DateTime.Parse(i["实际返回"].ToString());

                    if (se < st)
                    {
                        Log.Err(name + "-非法单据,返回时间小于开始时间");
                        continue;
                    }


                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaOriginaAdd(name, st, se, "外出");

                }
                DB.Commit();

                Log.Info(path + "-read done");
            }
            catch (Exception ex)
            {
                Log.Err("外出写入数据库出现问题" + ex.Message);
                DB.Rollback();
                throw;
            }


        }
        /// <summary>
        /// 考勤异常
        /// </summary>
        /// <param name="path"></param>
        private void OpenAddCheckinFile(string path)
        {//开始事务
            DB.BeginTransaction();
            try
            {
                var dt = new ExcelHelper(path).ExcelToDataTable("", true, true);

                var count = new Dictionary<string, int>();

                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {


                    var name = i["姓名"].ToString();


                    var st = DateTime.Parse(i["打卡异常时间"].ToString());


                    //月份不对.不处理
                    if (st.Month != monthCalendar1.SelectionStart.Month)
                        continue;

                    if (count.ContainsKey(name))
                    {
                        count[name] = count[name] + 1;
                        if (count[name] >= 4)
                        {
                            Log.Warn(name + "-超出3次补登,请注意");
                            continue;
                        }
                    }
                    else
                    {
                        count.Add(name, 1);
                    }

                    var r = i["上班或下班打卡异常"].ToString();
                    PersonInfo p;
                    if (DB.Persons.ContainsKey(name))
                        p = DB.Persons[name];
                    else
                    {
                        p = new PersonInfo(name);
                        DB.Persons.Add(name, p);
                    }

                    if (r == "上班")
                        st += p.WorkTimeClass.InTime;
                    else
                        st += p.WorkTimeClass.OutTime;

                    //写到表里

                    DB.OaOriginaAdd(name, st, st, "补登");
                }

                DB.Commit();

                Log.Info(path + "-read done");
            }
            catch (Exception ex)
            {
                Log.Warn("补登写入数据库出现问题" + ex.Message);
                DB.Rollback();
                throw;
            }


        }
        /// <summary>
        /// 出差
        /// </summary>
        /// <param name="path"></param>
        private void OpenOutworkFile(string path)
        {//开始事务
            DB.BeginTransaction();
            try
            {


                var dt = new ExcelHelper(path).ExcelToDataTable("", true, true);



                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();

                    if (i["实际开始"].ToString() == "")
                        continue;

                    var st = DateTime.Parse(i["实际开始"].ToString());
                    var se = DateTime.Parse(i["实际结束"].ToString());

                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaOriginaAdd(name, st, se, "出差");
                }

                DB.Commit();
                Log.Info(path + "-read done");
            }
            catch (Exception ex)
            {
                Log.Warn("出差写入数据库出现问题" + ex.Message);
                DB.Rollback();
                throw;
            }


        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="path"></param>
        private void OpenVacationFile(string path)
        {
            //开始事务
            DB.BeginTransaction();
            try
            {
                var dt = new ExcelHelper(path).ExcelToDataTable("", true, true);

                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();
                    var st = DateTime.Parse(i["开始时间"].ToString());
                    var se = DateTime.Parse(i["结束时间"].ToString());
                    var reson = i["请假类型"].ToString();

                    //写到表里
                    if (st.Month == monthCalendar1.SelectionStart.Month)
                        DB.OaOriginaAdd(name, st, se, reson);
                }
                DB.Commit();
                Log.Info(path + "-read done");
            }
            catch (Exception ex)
            {
                Log.Warn("假期写入数据库出现问题" + ex.Message);
                DB.Rollback();
                throw;
            }
        }
        #endregion



        public void WriteExcel(DataTable dt, string path)
        {



            if (File.Exists(path))
                File.Delete(path);


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
                    if (!dt.Rows[i][j].ToString().Contains("正常"))
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

            var p = DB.Persons[comboBox1.Text];
            

            //统计信息
            
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

            //得到oa数据
            oa_dataGridView2.DataSource = DB.GetSql("select * from oa where name ='" + p.Name + "'");

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

           
        }


        private void 打开考勤器文件ToolStripMenuItem_Click(object sender, EventArgs e)
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
            _http.Close();
        }

        private void 输出文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            //所有人遍历
            var count = 0;
            var dt = new DataTable();
            dt.Columns.Add("name");
            //加日期

            toolStripProgressBar1.Maximum = DB.Persons.Count;
            //对每人个进行遍历
            foreach (var p in DB.Persons)
            {
                //当前人名字
                var name = p.Value.Name;

                var dr = dt.NewRow();
                dr["name"] = name;
               // p.Value.GetData();

                //得到信息
                foreach (var c in p.Value.Checks)
                {

                    dr[c.Date.ToString()] = c.warninfo;


                }
                dt.Rows.Add(dr);

                count++;
                toolStripProgressBar1.Value = count;
            }

            WriteExcel(dt, outputfilename);

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


        private void Sendmail(PersonInfo p)
        {


            //合成文字
            try
            {
                if (p.Mail.IndexOf("@") == -1)
                {
                    throw new Exception("邮箱地址不合法");
                }

               // p.GetData();

                //统计信息
                var body = $"{p.Name}-考勤分析报表\r\n";

                body += p.GetText();


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
                Log.Info(p.Name + "-send mall done");
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
            foreach (var p in DB.Persons)
            {
                Sendmail(p.Value);
                k++;
                toolStripProgressBar1.Maximum = DB.Persons.Count;
                toolStripProgressBar1.Value = k;
            }

        }

        private void 向当前用户发送ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sendmail(DB.Persons[comboBox1.Text]);
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
            var s = new ShowData(DB.PersonInfos);
            s.Show();
        }


        private void 原始表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.CheckOriginalDt);
            s.Show();
        }

        private void oa表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sql = "select * from oa";
            var k = new ShowData(DB.GetSql(sql));
            k.Show();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://127.0.0.1:8080?name=" + comboBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dateview = new DataView(DB.CheckOriginalDt) { RowFilter = $"date ='{monthCalendar1.SelectionStart.Date}'" };//去重
            var pcount = dateview.ToTable(true, "name");
            var s = new ShowData(pcount);
            s.Show();
        }

        private void 删除考勤数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DB.DelOrigina();

        }

        private void 工作日设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new workdaysetting();
            f.Show();
        }

        private void 删除OA数据ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DB.DelOA();
        }

        private void 读取oa数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //得到前一天的数据
            oahelper.GetData(monthCalendar1.SelectionStart.Date);

        }
    }

}
