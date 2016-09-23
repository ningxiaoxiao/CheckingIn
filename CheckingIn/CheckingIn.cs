using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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

        public static CheckingIn inst;
        /// <summary>
        /// 有人出勤的日期
        /// </summary>
        private DataTable alldates;

        public CheckingIn()
        {
            inst = this;

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            openFileDialog1.FileOk += OpenFileDialog1_FileOk;
            listView2.RetrieveVirtualItem += ListView2_RetrieveVirtualItem;

            InstTable();

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

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                _openedFleName = openFileDialog1.FileName;
                var dt = ExcelToDs(openFileDialog1.FileName);

                //进行遍历处理 生成新的名
                foreach (DataRow i in dt.Tables[0].Rows)
                {
                    //读出时间
                    var time = DateTime.Parse(i["日期时间"].ToString());



                    //对时间进行处理 

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
                            xlsadd(i["name"].ToString(), (DateTime)i["start"], ((DateTime)i["start"]).TimeOfDay, i["reason"].ToString());
                            xlsadd(i["name"].ToString(), (DateTime)i["end"], ((DateTime)i["end"]).TimeOfDay, i["reason"].ToString());
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
                                xlsadd(i["name"].ToString(), s + new TimeSpan(d, 0, 0, 0), new TimeSpan(0, 18, 30, 0), i["reason"].ToString());
                            }

                            break;

                        default:
                            break;
                    }


                }
                //得到所有有人出勤的日期
                var dv = new DataView(_xlsdt);
                alldates = dv.ToTable(true, "date");

                var t = new Thread(Changedata);
                t.Start();
            }
            catch (Exception ex)
            {

                Console.Out.WriteLine(ex.ToString());
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

        public DataSet ExcelToDs(string path)
        {
            var strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + "Extended Properties=Excel 8.0;";
            var conn = new OleDbConnection(strConn);
            conn.Open();
            var strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [123$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
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

            //读出所有姓名
            var dv = new DataView(_xlsdt);
            var namedt = dv.ToTable(true, "name");


            toolStripProgressBar1.Maximum = namedt.Rows.Count;
            toolStripProgressBar1.Value = 0;

            //对每人个进行遍历
            foreach (DataRow r in namedt.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();
                comboBox1.Items.Add(n);


                toolStripProgressBar1.Value += 1;
            }
            comboBox1.SelectedIndex = 0;
            //输出文件

        }

        private void NewRecord(string name, object dt, object intime, object outtime, out TimeSpan worktime)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //对当前数据进行处理
            GetData(comboBox1.Text);



            //得到这个人所有的日期
            var dv = new DataView(_resultdt) { RowFilter = $"name = '{comboBox1.Text}'" };

            monthCalendar1.RemoveAllBoldedDates();
            foreach (DataRowView dr in dv)
            {
                monthCalendar1.AddBoldedDate((DateTime)dr.Row["date"]);
            }
            var v = monthCalendar1.BoldedDates[0];

            monthCalendar1.SetDate(v.AddMonths(1));
            monthCalendar1.SetDate(v);

            //得到相关提示记录
            dv = new DataView(_warnTable) { RowFilter = $"name = '{comboBox1.Text}'" };


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
                label4.Text += dv[0]["intime"].ToString() + "\r\n";
                label4.Text += dv[0]["outtime"].ToString() + "\r\n";
                label4.Text += dv[0]["worktime"].ToString() + "\r\n";
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
            //对所有有人出勤的日期进行遍历
            foreach (DataRow dater in alldates.Rows)
            {
                toolStripProgressBar1.Value += 1;
                //今日日期
                var date = dater["date"];

                //判断是不是工作日
                //如果有30个出勤,就算工作日
                var dateview = new DataView(_xlsdt) { RowFilter = $"date ='{date}'" };
                var isworkday = dateview.Count > 50;

                //得到这个人今天所有的打卡时间
                var timeview = new DataView(_xlsdt)
                {
                    RowFilter = $"name = '{name}' AND date = '{date}'",
                    Sort = "time asc" //从小到大
                };
                //无打卡记录
                if (timeview.Count == 0)
                {
                    if (isworkday)
                        Warn(name, date, "旷工");

                    continue;//当天没有记录 返回
                }

                //进行记录
                var intime = (TimeSpan)timeview[0].Row["time"];
                var outtime = (TimeSpan)timeview[timeview.Count - 1].Row["time"];
                TimeSpan wt;
                NewRecord(name, date, intime, outtime, out wt);


                //相关警告
                if (timeview.Count < 2)
                {
                    Warn(name, date, "少打卡");
                }
                else
                {

                    //todo 统计迟到时间
                    if (intime > new TimeSpan(0, 9, 30, 0))
                    {
                        Warn(name, date, "迟到");
                    }

                    //todo 早退

                    //两次打卡完整 

                    if (wt < new TimeSpan(0, 9, 0, 0))
                    {
                        Warn(name, date, "少工时");
                    }

                    //todo 统计加班时间
                    if (!isworkday)
                        Warn(name, date, "疑似加班");

                }


            }
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }


        private void 增加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new oadata();
            f.Show();
        }
    }
}
