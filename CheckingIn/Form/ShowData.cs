using System.Data;
using System.Windows.Forms;

namespace CheckingIn
{
    public partial class ShowData : Form
    {

        public ShowData(object dt)
        {
            InitializeComponent();
            dataGridView1.DataSource = dt;
            Text += "-"+((DataTable) dt).TableName;
        }
    }
}
