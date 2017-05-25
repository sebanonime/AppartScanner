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

        public static void Debug(string format)
        {
            logger.Debug(format);
        }

        public static void Info(string format)
        {
            logger.Info(format);
        }

        public static void Error(string format)
        {
            logger.Error(format);
        }
    }
}
