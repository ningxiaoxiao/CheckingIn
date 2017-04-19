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
            

            dataGridView1.DataSource = DB.OaOriginaDt;

        }

        private void button_add_Click(object sender, EventArgs e)
        {

            //数据合法性
            if (textBox_name.Text == "" || dateTimePicker_start.Value > dateTimePicker_end.Value)
                return;


            //写到表里

            var o = new Dos.Model.oa()
            {
                name = textBox_name.Text,
                date = dateTimePicker_start.Value.Date,
                start = dateTimePicker_start.Value,
                end = dateTimePicker_end.Value.AddMinutes(1),
                reason = _curReason,
                subreason = ""

            };

            DB.Context.Insert(o);


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

        }
    }
}
