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

            dataGridView1.DataSource = DB.OaOriginaDt;

        }

        private void button_add_Click(object sender, EventArgs e)
        {

            //数据合法性
            if (comboBox_name.Text == "" || dateTimePicker_start.Value > dateTimePicker_end.Value)
                return;


            //写到表里



            DB.OaOriginaAdd(comboBox_name.Text, dateTimePicker_start.Value, dateTimePicker_end.Value.AddMinutes(1), _curReason);
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

        private void dateTimePicker_start_ValueChanged(object sender, EventArgs e)
        {
            if (_curReason == "补登")
            {
                dateTimePicker_end.Value = dateTimePicker_start.Value;
            }
        }

        private void dateTimePicker_end_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker_end.Value < dateTimePicker_start.Value)
                dateTimePicker_end.Value = dateTimePicker_start.Value;
        }

        private void oadata_FormClosed(object sender, FormClosedEventArgs e)
        {
            DB.Readoa();
        }
    }
}
