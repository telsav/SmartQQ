using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using Console = Colorful.Console;
using System.Text;

namespace SmartQQ
{
    public sealed class Logger
    {

        private static Logger instance = null;

        private static object loggerLock = new object();
        public Logger()
        {
            LogFileExtension = ".log";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            OutputType = OutputType.Console;
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    lock (loggerLock)
                    {
                        instance = new Logger();
                    }
                return instance;
            }
        }

        public StreamWriter Writer { get; set; }

        public OutputType OutputType { get; set; }

        public string LogPath
        {
            get
            {
                string basePath = string.Empty;
                try
                {
                    basePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
                    basePath = Path.Combine(basePath, "Log");
                    if (Directory.Exists(basePath) == false)
                        Directory.CreateDirectory(basePath);
                }
                catch
                {
                    basePath = @"c:\temp";
                    if (Directory.Exists(basePath) == false)
                        Directory.CreateDirectory(basePath);
                }
                return basePath;
            }
        }

        public string LogFileName
        {
            get
            {
                string logFile = string.Empty;
                try
                {
                    logFile = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
                    logFile = logFile + "_Sevice_" + DateTime.Now.ToString("yyyyMMdd");
                }
                catch
                {
                    logFile = "PPTV_Sevice_" + DateTime.Now.ToString("yyyyMMdd");
                }
                return logFile;
            }
        }

        public string LogFileExtension { get; set; }

        public string LogFile { get { return LogFileName + LogFileExtension; } }

        public string LogFullPath { get { return Path.Combine(LogPath, LogFile); } }

        public bool LogExists { get { return File.Exists(LogFullPath); } }

        public void Error(string message, string module)=>WriteEntry(message, LogType.Error, module);
        public void Error(string message)=> WriteEntry(message, LogType.Error);


        public void Error(Exception ex, string module)
        {
            string msg = string.Empty;
            for (var ee = ex; ee != null; ee = ee.InnerException)
            {
                msg += ee.Message + Environment.NewLine;
                msg += ee.StackTrace + Environment.NewLine;
            }
            WriteEntry(msg, LogType.Error, module);
        }
        public void Error(Exception ex)
        {
            string msg = string.Empty;
            for (var ee = ex; ee != null; ee = ee.InnerException)
            {
                msg += ee.Message + Environment.NewLine;
                msg += ee.StackTrace + Environment.NewLine;
            }
            WriteEntry(msg, LogType.Error);
        }

        public void Warning(string message, string module)=> WriteEntry(message, LogType.Warning, module);
        public void Warning(string message) => WriteEntry(message, LogType.Warning);


        public void Info(string message, string module)=> WriteEntry(message, LogType.Info, module);
        public void Info(string message)=> WriteEntry(message, LogType.Info);


        public void Debug(string message, string module) => WriteEntry(message, LogType.Debug, module);
        public void Debug(string message) => WriteEntry(message, LogType.Debug);

        private void WriteEntry(string message, LogType type, string module)
        {
            try
            {
                string log = string.Format("{0},{1},{2},{3}",
                                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                      type.ToString(),
                                      module,
                                      message);
                if (this.OutputType == OutputType.Text)
                {
                    File.AppendAllText(Instance.LogFullPath, log);
                }
                else if (this.OutputType == OutputType.Console)
                {
                    if (type == LogType.Warning)
                    {
                        Console.WriteLine(log, Color.Yellow);
                    }
                    else if (type == LogType.Error)
                    {
                        Console.WriteLine(log, Color.Red);
                    }
                    else
                    {
                        Console.WriteLine(log);
                    }
                }
            }
            catch
            {
                //throw;
            }

        }
        private void WriteEntry(string message, LogType type)
        {
            try
            {

                string log = string.Format("{0},{1},{2},{3}",
                                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                      type.ToString(),
                                      System.Environment.MachineName,
                                      message);
                log = log + Environment.NewLine;

                if (this.OutputType == OutputType.Text)
                {
                    File.AppendAllText(Instance.LogFullPath, log);
                }
                else if (this.OutputType == OutputType.Console)
                {
                    if (type == LogType.Warning)
                    {
                        Console.WriteLine(log, Color.Yellow);
                    }
                    else if (type == LogType.Error)
                    {
                        Console.WriteLine(log, Color.Red);
                    }
                    else
                    {
                        Console.WriteLine(log);
                    }
                }
            }
            catch
            {
                return;
                //throw;
            }

        }
    }

    public enum LogType
    {
        Info,
        Error,
        Warning,
        Trace,
        Debug,
        None
    }

    public enum OutputType
    {
        Text,
        Console
    }
}
