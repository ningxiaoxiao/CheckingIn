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
        private string Jsonyear => DateTime.Now.Year.ToString(); 
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

            var f = new StreamReader("JsonData\\worktimeclass.json").ReadToEnd();
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
            try
            {
              
                workdaysjson = JsonMapper.ToObject(htmlstr);
                workdaysjson = workdaysjson["data"][Jsonyear];
                //写入文件
                File.WriteAllText("JsonData\\notworkdays.json", htmlstr);
            }
            catch (Exception )
            {
                Log.Err("非工作日调用接口有问题,使用上一次的有效文件");

                htmlstr= File.OpenText("JsonData\\notworkdays.json").ReadToEnd();
                workdaysjson = JsonMapper.ToObject(htmlstr);
                workdaysjson = workdaysjson["data"][Jsonyear];
            }


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

                case "选择班次文件":
                    OpenWorkTimeClassFile(openFileDialog1.FileName);
                    break;
                case "选择邮箱文件":
                    OpenMailFile(openFileDialog1.FileName);
                    break;

            }

        }


        /// <summary>
        /// 班次表
        /// </summary>
        /// <param name="path"></param>
        private void OpenWorkTimeClassFile(string path)
        {
            //开始事务
            var tran = DB.Context.BeginTransaction();
            var updatacount = 0;
            var addcount = 0;
            try
            {
                //读出文件
                var dt = new ExcelHelper(path).ExcelToDataTable("", true);
                var dbdt = DB.Context.From<Dos.Model.person>().ToDataTable();
                var delcount = 0;
                //清理文件中没有的人
                foreach (DataRow i in dbdt.Rows)
                {
                    var name = i["name"].ToString();
                    var cn = i["worktimeclass"].ToString();

                    var rs = dt.Select($"姓名 ='{name}'");
                    if (rs.Length == 0)
                    {
                        //没有这个人了.删除
                        DB.Context.Delete<Dos.Model.person>(tran, p => p.name == name);
                        delcount++;

                    }
                }

                Log.Info("delcount=" + delcount);


                //进行遍历处理    
                foreach (DataRow i in dt.Rows)
                {
                    var name = i["姓名"].ToString();
                    var classname = i["对应时段"].ToString();

                    var rs = dbdt.Select($"name ='{name}'");

                    if (rs.Length == 0)
                    {
                        var p = new Dos.Model.person()
                        {
                            name = name,
                            worktimeclass = classname,
                            password = "123456",

                        };

                        DB.Context.Insert(tran, p);
                        addcount++;
                    }
                    else if (rs[0]["worktimeclass"].ToString() != classname)
                    {

                        DB.Context.Update<Dos.Model.person>(tran, Dos.Model.person._.worktimeclass, classname, Dos.Model.person._.name == name);
                        Log.Info($"updata name={name},wt={classname}");
                        updatacount++;
                    }


                }

                tran.Commit();

                Log.Info($"worktimeclass done,add={addcount},updatacount={updatacount}");
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

            //todo  发送邮件


        }





        public void WriteDtToExcelFile(DataTable dt, string path)
        {
            StringToFile(DtToString(dt), path);
        }

        public string DtToString(DataTable dt)
        {
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
            return sb.ToString();
        }

        public void StringToFile(string s, string path)
        {

            if (File.Exists(path))
                File.Delete(path);

            var sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));//打开写文件流
            sw.Write(s);//文件流写入内容
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


        public string GetOutXlsString(int year, int month)
        {
            return DtToString(GetOutXlsDt(year,month));
        }

        private DataTable GetOutXlsDt(int year,int month)
        {
            //所有人遍历
            var count = 0;
            var dt = new DataTable();
            dt.Columns.Add("name");
            //统计列

            dt.Columns.Add("剩余假期");
            dt.Columns.Add("工作天数");
            dt.Columns.Add("工作时间");
            dt.Columns.Add("迟到早退");
            dt.Columns.Add("使用调休假期");
            dt.Columns.Add("使用扣薪假期");
            dt.Columns.Add("加班");
            dt.Columns.Add("出差");



            //生成日期列



            //日期
            var n = new DateTime(year, month, 1);//这个月的第一天
            while (n.Month == month)
            {
                dt.Columns.Add(n.ToShortDateString());
                n = n.AddDays(1);
            }



            toolStripProgressBar1.Maximum = DB.Persons.Count;
            //对每人个进行遍历
            foreach (var p in DB.Persons)
            {
                //当前人名字
                var name = p.Value.Name;

                var drup = dt.NewRow();
                var drdown = dt.NewRow();
                drup["name"] = name + "上午";
                drdown["name"] = name + "下午";

                p.Value.GetData(year,month);

                //统计信息
                drup["剩余假期"] = p.Value.CanUseHolidayHour.TotalHours.ToString("0.#") + "小时";
                drup["工作时间"] = p.Value.WorkTime.TotalHours.ToString("0.#") + "/" + p.Value.ShoudWorkDayCount * 8 + "小时";
                drup["工作天数"] = p.Value.ShoudWorkDayCount - p.Value.WarnDayCount + "/" + p.Value.ShoudWorkDayCount + "天";

                drup["迟到早退"] = p.Value.DelayTime.TotalMinutes.ToString("0.# '分钟'");

                drup["使用调休假期"] = p.Value.useHolidayhours.TotalHours.ToString("0.#") + "小时";
                drup["使用扣薪假期"] = p.Value.NoPayHolidaysHours.TotalHours.ToString("0.#") + "小时";
                drup["加班"] = p.Value.OverWorkTime.TotalHours.ToString("0.# '小时'");
                drup["出差"] = p.Value.Travel + "天";



                //得到信息
                foreach (var c in p.Value.Checks)
                {
                    if (c.Date >= DateTime.Now) continue;
                    var upstr = "√";
                    var donwstr = "√";


                    var warninfos = c.warninfo.Split(' ');

                    foreach (var wraninfo in warninfos)
                    {

                        if (wraninfo.Contains("分钟"))
                        {
                            if (p.Value.DelayTime > new TimeSpan(0, 30, 0))//如果总时间小于30 就去掉迟到时间
                            {
                                if (wraninfo.Contains("迟到"))
                                    upstr = wraninfo;
                                else if (wraninfo.Contains("早退"))
                                    donwstr = wraninfo;
                            }
                        }
                        if (wraninfo.Contains("上班未打卡"))
                        {
                            upstr = "上未";
                        }

                        if (wraninfo.Contains("下班未打卡"))
                        {
                            donwstr = "下未";
                        }

                        if (wraninfo.Contains("旷工"))
                        {
                            upstr = "旷工";
                            donwstr = "旷工";
                        }

                        if (c.Info.Contains("休假"))
                        {
                            upstr = c.Info;
                            donwstr = c.Info;
                        }


                        if (!c.Date.IsWorkDay)
                        {
                            upstr = "圆";
                            donwstr = "圆";
                        }
                        drup[c.Date.ToString()] = upstr;
                        drdown[c.Date.ToString()] = donwstr;
                    }
                }
                dt.Rows.Add(drup);
                dt.Rows.Add(drdown);

                count++;
                toolStripProgressBar1.Value = count;
            }
            return dt;
        }

        public void GetOutXlsFile()
        {

            WriteDtToExcelFile(GetOutXlsDt(DateTime.Now.Year,DateTime.Now.Month), outputfilename);
        }

        private void 输出文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            GetOutXlsFile();


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


    }

}
