using System;
using System.Windows.Forms;

namespace CheckingIn
{
    public partial class oadata : Form
    {
        private string _curReason = "出差";
        public oadata()
        {
            InitializeComponent();

            var cm = new object[CheckingIn.inst.comboBox1.Items.Count];
            CheckingIn.inst.comboBox1.Items.CopyTo(cm, 0);
            comboBox_name.Items.AddRange(cm);
            if (comboBox_name.Items.Count > 0)
                comboBox_name.SelectedIndex = 0;
            listView1.VirtualListSize = CheckingIn.inst.OAdt.Rows.Count;
            listView1.Update();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            //写到表里
            AddRecord(comboBox_name.Text, dateTimePicker_start.Value, dateTimePicker_end.Value, _curReason);
            listView1.VirtualListSize = CheckingIn.inst.OAdt.Rows.Count;
            listView1.Update();
        }
        private void AddRecord(string name, DateTime s, DateTime e, string r)
        {
            var nr = CheckingIn.inst.OAdt.NewRow();
            nr["name"] = name;
            nr["start"] = s;
            nr["end"] = e.AddMinutes(1);
            nr["reason"] = r;
            CheckingIn.inst.OAdt.Rows.Add(nr);
        }

        private void oadata_FormClosed(object sender, FormClosedEventArgs e)
        {
            //todo 写到文档
        }

        private void oadata_Load(object sender, EventArgs e)
        {
            //todo 读取存档

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                _curReason = rb.Text;

                dateTimePicker_end.Visible = rb.Text != "补登";
            }
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(new[] {
                CheckingIn.inst.OAdt.Rows[e.ItemIndex]["name"].ToString(),
                CheckingIn.inst.OAdt.Rows[e.ItemIndex]["start"].ToString(),
                CheckingIn.inst.OAdt.Rows[e.ItemIndex]["end"].ToString(),
                CheckingIn.inst.OAdt.Rows[e.ItemIndex]["reason"].ToString(),
            });
        }

        private void dateTimePicker_start_ValueChanged(object sender, EventArgs e)
        {
            if (_curReason == "补登")
            {
                dateTimePicker_end.Value = dateTimePicker_start.Value;
            }
        }
    }
}
