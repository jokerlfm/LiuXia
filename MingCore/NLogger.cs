using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MingCore
{
    public class NLogger
    {
        public static void InitLoggers(string pmLogPath,bool pmRuntimeLogger,bool pmNetworkLogger)
        {
            if (pmRuntimeLogger)
            {
                if (runtimeLogger == null)
                {
                    runtimeLogger = new MixLogger(pmLogPath, "Runtime");
                }
            }
            if (pmNetworkLogger)
            {
                if (networkLogger == null)
                {
                    networkLogger = new MixLogger(pmLogPath, "Network");
                }
            }
        }
        #region declaration
        private static MixLogger runtimeLogger = null;
        public static MixLogger RuntimeLogger
        {
            get
            {
                return runtimeLogger;
            }
        }

        private static MixLogger networkLogger = null;
        public static MixLogger NetworkLogger
        {
            get
            {
                return networkLogger;
            }
        }
        #endregion
    }

    public class MixLogger
    {
        public MixLogger(string pmLogPath, string pmLogName)
        {
            this.logPath = pmLogPath;
            this.logName = pmLogName;
            this.logQueue = new Queue<string>(1000);
            this.logger = log4net.LogManager.GetLogger(this.GenerateCurrentLog4NetRepository(), pmLogName);
        }

        #region declaration
        private string logPath = "";
        public string LogPath
        {
            get
            {
                return this.logPath;
            }
        }

        private string logName = "";
        public string LogName
        {
            get
            {
                return this.logName;
            }
        }

        private log4net.ILog logger = null;
        public log4net.ILog Logger
        {
            get
            {
                return logger;
            }
        }

        private Queue<string> logQueue = null;
        #endregion

        #region business
        private string GenerateCurrentLog4NetRepository()
        {
            string result = "";

            ///Appender
            log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();

            appender.AppendToFile = true;
            appender.File = this.logPath + "\\" + this.logName;
            appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Composite;
            appender.DatePattern = ".dd";
            appender.MaxSizeRollBackups = 100;
            appender.MaximumFileSize = "100MB";
            appender.CountDirection = 1;
            appender.ImmediateFlush = true;
            appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();

            ///layout  
            //log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%message%newline");
            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%-5level:%message%newline");
            layout.Header = "------ New session ------" + Environment.NewLine;
            layout.Footer = "------ End session ------" + Environment.NewLine;
            //              
            appender.Layout = layout;
            appender.ActivateOptions();
            //  
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.CreateRepository(this.logName + "Repository");
            log4net.Config.BasicConfigurator.Configure(repository, appender);
            result = repository.Name;

            return result;
        }

        private void EnqueueLog(string pmContent)
        {
            if (this.logQueue.Count > 500)
            {
                this.logQueue.Dequeue();
            }
            this.logQueue.Enqueue(pmContent);
        }

        public void Debug(string pmContent)
        {
            this.logger.Debug(pmContent);
            this.EnqueueLog(pmContent);
        }

        public void Info(string pmContent)
        {
            this.logger.Info(pmContent);
            this.EnqueueLog(pmContent);
        }

        public void Warn(string pmContent)
        {
            this.logger.Warn(pmContent);
            this.EnqueueLog(pmContent);
        }

        public void Error(string pmContent)
        {
            this.logger.Error(pmContent);
            this.EnqueueLog(pmContent);
        }

        public string DequeueLog()
        {
            return this.logQueue.Dequeue();
        }
        #endregion
    }
}