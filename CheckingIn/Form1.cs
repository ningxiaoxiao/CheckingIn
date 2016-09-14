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

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            openFileDialog1.FileOk += OpenFileDialog1_FileOk;
            listView1.RetrieveVirtualItem += ListView1_RetrieveVirtualItem;
            listView2.RetrieveVirtualItem += ListView2_RetrieveVirtualItem;
        }

        private void ListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                _listdt.Rows[e.ItemIndex]["name"].ToString(),
                _listdt.Rows[e.ItemIndex]["date"].ToString(),
                _listdt.Rows[e.ItemIndex]["time"].ToString(),
            });
        }

        private void ListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                _listResultdt.Rows[e.ItemIndex]["name"].ToString(),
                _listResultdt.Rows[e.ItemIndex]["date"].ToString(),
                _listResultdt.Rows[e.ItemIndex]["intime"].ToString(),
                _listResultdt.Rows[e.ItemIndex]["outtime"].ToString(),
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
                    newr["date"] = time.Date;
                    newr["time"] = time.TimeOfDay;
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


        private void Changedata()
        {
            //结果表
            _resultdt = new DataTable();

            _resultdt.Columns.Add("name", typeof(string));
            _resultdt.Columns.Add("date", typeof(DateTime));
            _resultdt.Columns.Add("intime", typeof(TimeSpan));
            _resultdt.Columns.Add("outtime", typeof(TimeSpan));




            //读出所有姓名
            var dv = new DataView(_xlsdt);
            var namedt = dv.ToTable(true, "name");
            //生成所有当月工作日

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
                    //得到这个人今天所有的时间
                    var date = dater["date"];

                    var timeview = new DataView(_xlsdt) { RowFilter = $"name = '{n}' AND date = '{date}'" };
                    var t = timeview.ToTable();

                    var times = new ArrayList();

                    foreach (DataRow timer in t.Rows)
                    {
                        times.Add(timer["time"]);
                    }

                    if (times.Count == 0) continue;//当天没有记录 返回

                    times.Sort();
                    //进行记录
                    var rr = _resultdt.NewRow();
                    rr["name"] = n;
                    rr["date"] = date;
                    rr["intime"] = times[0];
                    rr["outtime"] = times[times.Count - 1];
                    _resultdt.Rows.Add(rr);


                    // Console.Out.WriteLine("{0}:{1}:{2}:{3}", n, date, times[0], times[times.Count - 1]);
                }
                progressBar1.Value += 1;
            }
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //得到这个人所有的日期
            var dv = new DataView(_resultdt) { RowFilter = $"name = '{comboBox1.Text}'" };

            var days = dv.ToTable();

            monthCalendar1.RemoveAllBoldedDates();
            foreach (DataRow dr in days.Rows)
            {
                monthCalendar1.AddBoldedDate((DateTime)dr["date"]);
            }
            var v = monthCalendar1.BoldedDates[0];

            monthCalendar1.SetDate(v.AddMonths(1));
            monthCalendar1.SetDate(v);


        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
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
