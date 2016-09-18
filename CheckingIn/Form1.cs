using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Windows.Forms;

namespace CheckingIn
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 处理后的表格
        /// </summary>
        private DataTable _resultdt;
        /// <summary>
        /// 原始表格
        /// </summary>
        private DataTable _xlsdt;
        /// <summary>
        /// 列表使用的
        /// </summary>
        private DataTable _listResultdt;
        /// <summary>
        /// 列表使用的
        /// </summary>
        private DataTable _listdt;

        private DataTable _warnTable;


        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            openFileDialog1.FileOk += OpenFileDialog1_FileOk;
            listView1.RetrieveVirtualItem += ListView1_RetrieveVirtualItem;
            listView2.RetrieveVirtualItem += ListView2_RetrieveVirtualItem;

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

        }

        private void ListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                _listdt.Rows[e.ItemIndex]["name"].ToString(),
                ((DateTime)_listdt.Rows[e.ItemIndex]["date"]).ToShortDateString(),
                _listdt.Rows[e.ItemIndex]["time"].ToString(),
            });
        }

        private void ListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                _listResultdt.Rows[e.ItemIndex]["name"].ToString(),
                ((DateTime)_listResultdt.Rows[e.ItemIndex]["date"]).ToShortDateString(),
                _listResultdt.Rows[e.ItemIndex]["intime"].ToString(),
                _listResultdt.Rows[e.ItemIndex]["outtime"].ToString(),
                _listResultdt.Rows[e.ItemIndex]["worktime"].ToString(),
            });
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var dt = ExcelToDs(openFileDialog1.FileName);


                //二次处理的表

                _xlsdt = new DataTable();

                _xlsdt.Columns.Add("name", typeof(string));
                _xlsdt.Columns.Add("date", typeof(DateTime));
                _xlsdt.Columns.Add("time", typeof(TimeSpan));

                //进行遍历处理 生成新的名
                foreach (DataRow i in dt.Tables[0].Rows)
                {

                    //读出时间
                    var time = DateTime.Parse(i["日期时间"].ToString());
                    //增加新记录
                    var newr = _xlsdt.NewRow();
                    newr["name"] = i["姓名"];
                    //对时间进行处理 

                    //如果 时间是 05:00前的 就把日期算到前一天上面去

                    if (time.TimeOfDay < new TimeSpan(5, 0, 0))
                    {
                        time = time.AddDays(-1);
                        newr["time"] = time.TimeOfDay.Add(new TimeSpan(1, 0, 0, 0));//时间值多一天

                        Warn(i["姓名"].ToString(), time, "有凌晨打卡");
                    }
                    else
                    {
                        newr["time"] = time.TimeOfDay;
                    }

                    newr["date"] = time.Date;

                    _xlsdt.Rows.Add(newr);
                }



                var t = new Thread(Changedata);
                t.Start();
            }
            catch (Exception ex)
            {

                Console.Out.WriteLine(ex.ToString());
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        public DataSet ExcelToDs(string Path)
        {
            var strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
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

            progressBar1.Maximum = namedt.Rows.Count;
            progressBar1.Value = 0;

            //对每人个进行遍历
            foreach (DataRow r in namedt.Rows)
            {
                //当前人名字
                var n = r["name"].ToString();
                comboBox1.Items.Add(n);
                //读出这个人所有记录
                var rs = _xlsdt.Select("name = '" + n + "'");

                //得到所有有人出勤的日期
                var datedt = dv.ToTable(true, "date");


                foreach (DataRow dater in datedt.Rows)
                {
                    //今日日期
                    var date = dater["date"];

                    //判断是不是工作日
                    //如果有30个出勤,就算工作日
                    //取出今天工作的人
                    var dateview = new DataView(_xlsdt) { RowFilter = $"date ='{date}'" };
                    var isworkday = dateview.Count > 50;

                    if (!isworkday)
                    {
                        Warn(n, date, "出勤人员少于50人,非工作日");
                    }

                    
                    //得到这个人今天所有的打卡时间
                    var timeview = new DataView(_xlsdt)
                    {
                        RowFilter = $"name = '{n}' AND date = '{date}'",
                        Sort = "time asc" //从小到大
                    };

                    

                    if (timeview.Count == 0)
                    {
                        if (isworkday)
                            Warn(n, date, "工作日未出勤");

                        continue;//当天没有记录 返回
                    }


                    //进行记录
                    TimeSpan wt;
                    NewRecord(n, date, timeview[0].Row["time"], timeview[timeview.Count - 1].Row["time"], out wt);


                    //相关警告
                    if (timeview.Count < 2)
                    {
                        Warn(n, date, "打卡次数少于2次");
                    }
                    else if (wt < new TimeSpan(0, 9, 0, 0))
                    {
                        Warn(n, date, "工作时间少于9小时");
                    }

                    if (!isworkday)
                        Warn(n, date, "非工作日出勤");

                    // Console.Out.WriteLine("{0}:{1}:{2}:{3}", n, date, times[0], times[times.Count - 1]);
                }
                progressBar1.Value += 1;
            }
            comboBox1.SelectedIndex = 0;
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
                        i.Row["txt"].ToString(),
                    })
                    );
            }


        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {

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
            //得到这个用户 当天的记录
            var dv = new DataView(_resultdt) { RowFilter = $"name = '{comboBox1.Text}' AND date = '{e.Start}'" };
            _listResultdt = dv.ToTable();
            listView1.VirtualListSize = _listResultdt.Rows.Count;
            listView1.Invalidate();

            //原来的记录
            dv = new DataView(_xlsdt) { RowFilter = $"name = '{comboBox1.Text}' AND date = '{e.Start}'" };
            _listdt = dv.ToTable();
            listView2.VirtualListSize = _listdt.Rows.Count;
            listView2.Invalidate();

        }
    }
}
