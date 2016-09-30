using log4net;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace CheckingIn
{
    public static class Log
    {
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static ListView _ls;

        public static void Creat(ListView ls)
        {
            _ls = ls;
        }
        public static void info(object o)
        {
            info(o.ToString());
        }
        public static void info(string str)
        {
            logger.Info(str);
            log("info", str, Color.Black);
        }
        public static void warn(string str)
        {
            logger.Warn(str);
            log("warn", str, Color.Orange);
        }
        public static void err(string str)
        {
            logger.Error(str);
            log("err", str, Color.Red);
        }
        private static void log(string lx, string nr, Color c)
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
