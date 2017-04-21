using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace oatoolDBhelper
{
   
    class log
    {
        public static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
