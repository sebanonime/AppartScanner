using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appart
{
    public static class Logger
    {
        static ILog logger;

        public static bool IsDebug
        {
            get
            {
                return logger.IsDebugEnabled;
            }
        }

        static Logger()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("Log4net.config"));
            logger = LogManager.GetLogger("AppartScanner");
        }

        public static void Debug(string format, params object[] args)
        {
            logger.DebugFormat(format, args);
        }

        public static void Info(string format, params object[] args)
        {
            logger.InfoFormat(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            logger.ErrorFormat(format, args);
        }
    }
}
