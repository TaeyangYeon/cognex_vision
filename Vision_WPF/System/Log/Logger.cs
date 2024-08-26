using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SystemConf;
using UI;

namespace Log
{
    public class LogEventArgs : EventArgs
    {
        public LogEntity log { get; set; }
    }
    public class Logger
    {
        private static Logger logger;
        private static readonly object _lock = new object();
        private readonly string logPath;

        public event EventHandler<LogEventArgs> onLogAdded;
        
        private Logger()
        {
            FileInfo basePath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
            logPath = basePath.Directory.FullName + SystemManagerConstValue.SYSTEM_LOG_PATH;
        }

        public static Logger instance()
        {
            if (logger == null)
            {
                lock (_lock)
                {
                    if (logger == null)
                    {
                        logger = new Logger();
                    }
                }
            }
            return logger;
        }

        public string getLogFilePath()
        {
            string datePath = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                string fullPath = Path.Combine(logPath, datePath);

                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

                return Path.Combine(fullPath, "log.csv");
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public void pushLog(LogLevel level, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", string _location = "")
        {
            string location = (string.IsNullOrWhiteSpace(_location)) ?
                Path.GetFullPath(filePath) + "." + memberName :
                _location;

            var log = new LogEntity
            {
                level = level,
                location = location,
                message = message
            };

            try
            {
                writeLog(log);
                onLogAdded?.Invoke(this, new LogEventArgs { log = log });
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        private void writeLog(LogEntity log)
        {
            try
            {
                string logPath = getLogFilePath();
                bool fileExists = File.Exists(logPath);

                using (StreamWriter sw = new StreamWriter(logPath, true))
                {
                    if (!fileExists)
                    {
                        sw.WriteLine("Time,Level,Location,Message");
                    }
                    sw.WriteLine(log.ToString());
                }
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public List<LogEntity> getLogs(string date)
        {
            string logFilePath = Path.Combine(logPath, date, "log.csv");
            var logs = new List<LogEntity>();

            if (File.Exists(logFilePath))
            {
                logs = File.ReadAllLines(logFilePath)
                    .Skip(1)
                    .Select(lines =>
                    {
                        string[] values = lines.Split(',');
                        return new LogEntity(values[0], (LogLevel)Enum.Parse(typeof(LogLevel), values[1]), values[2], values[3]);

                    })
                    .ToList();
            }
            else
            {
                throw new IOException();
            }
            return logs;
        }

    }
}
