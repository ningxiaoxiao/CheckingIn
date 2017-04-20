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
        public JsonData worktimeclassjson { get; }
        private const string Jsonyear = "2017";
        public CheckingIn()
        {
            Inst = this;

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            openFileDialog1.FileOk += OpenFileDialog1_FileOk;

            Log.Creat(listView_log);



            //启动http服务
            _http = new HttpSever();

            //得到非工作日
            GetNoWorkday();

            var f = new StreamReader("worktimeclass.json").ReadToEnd();
            worktimeclassjson = JsonMapper.ToObject(f);




            DB.Creat();





        }
        private void GetNoWorkday()
        {
            var h = new HttpHelper();
            var i = new HttpItem()
            {
                URL = $"http://tool.bitefu.net/jiari/vip.php?d={Jsonyear}&type=0&apikey=123456",
                IsToLower = false,
            };


            var htmlstr = h.GetHtml(i).Html;
            workdaysjson = JsonMapper.ToObject(htmlstr);
            workdaysjson = workdaysjson["data"][Jsonyear];
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

            DB.Context.DeleteAll<Dos.Model.original>();
            var tran = DB.Context.BeginTransaction();
            try
            {
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

                    var o = new Dos.Model.original()
                    {
                        name = i["姓名"].ToString(),
                        date = time.Date,
                        time = tt.Ticks,

                    };

                    DB.Context.Insert(tran, o);

                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Log.Err("读取考勤器文件出错." + ex.Message);
            }


            Log.Info("读取考勤器文件完成");
        }

        /// <summary>
        /// 班次表
        /// </summary>
        /// <param name="path"></param>
        private void OpenWorkTimeClassFile(string path)
        {


            var pdt = DB.Context.From<Dos.Model.person>().ToDataTable();

            //开始事务
            var tran = DB.Context.BeginTransaction();

            try
            {
                //读出文件
                var dt = new ExcelHelper(path).ExcelToDataTable("", true);
                //进行遍历处理    
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();
                    var classname = i["对应时段"].ToString();

                    var rs = pdt.Select($"name ='{name}'");

                    if (rs.Length == 0)
                    {
                        var p = new Dos.Model.person()
                        {
                            name = name,
                            worktimeclass = classname,
                            password = "123456",

                        };

                        DB.Context.Insert(tran,p);
                    }
                    else if (rs[0]["worktimeclass"].ToString() != classname)
                    {

                        DB.Context.Update<Dos.Model.person>(tran,Dos.Model.person._.worktimeclass, classname, Dos.Model.person._.name == name);
                    }


                }

                tran.Commit();

                Log.Info("worktimeclass done");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Log.Err(ex.Message);
            }
        }
        /// <summary>
        /// 邮件表
        /// </summary>
        /// <param name="path"></param>
        private void OpenMailFile(string path)
        {
            var dt = new ExcelHelper(path).ExcelToDataTable("", true);



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
                //todo 
                Log.Info("mail done");
            }
            catch (Exception ex)
            {


                throw ex;
            }
        }





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



        private void 个人信息表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.Context.From<Dos.Model.person>().ToDataTable());
            s.Show();
        }


        private void 原始表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.Context.From<Dos.Model.original>().ToDataTable());
            s.Show();
        }

        private void oa表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = new ShowData(DB.Context.From<Dos.Model.oa>().ToDataTable());
            s.Show();
        }



        private void 删除考勤数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var i = DB.Context.DeleteAll<Dos.Model.original>();
            Log.Info("delete original,count=" + i);

        }

        private void 工作日设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new workdaysetting();
            f.Show();
        }

        private void 删除OA数据ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var i = DB.Context.DeleteAll<Dos.Model.oa>();
            Log.Info("delete oa,count=" + i);
        }

        private void 读取oa数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var t = DateTime.Today;
            t = t.AddDays(-t.Day + 1);


            //得到当月第一天
            oahelper.GetData(t);

        }
    }

}
