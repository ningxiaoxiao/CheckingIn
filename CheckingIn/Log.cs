using log4net;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace CheckingIn
{
    public static class Log
    {
        public static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ListView _ls;

        public static void Creat(ListView ls)
        {
            _ls = ls;
        }
        public static void Info(object o)
        {
            Info(o.ToString());
        }
        public static void Info(string str)
        {
            _logger.Info(str);
            AddListItem("Info", str, Color.Black);
        }
        public static void Warn(string str)
        {
            _logger.Warn(str);
            AddListItem("Warn", str, Color.Orange);
        }
        public static void Err(string str)
        {
            _logger.Error(str);
            AddListItem("err", str, Color.Red);
        }
        private static void AddListItem(string lx, string nr, Color c)
        {

            if (_ls.Items.Count > 1000)
            {
                _ls.Items.Clear();
            }

            var lvi = new ListViewItem(new string[] { lx, nr }) { ForeColor = c };
            _ls.Items.Insert(0, lvi);
        }
    }
}
