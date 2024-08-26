using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public class LogEntity
    {
        public string time { get; set; } = DateTime.Now.ToString("HH:mm:ss.fff");
        public LogLevel level { get; set; }
        public string location { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return $"{time},{level},{location},{message}";
        }

        public LogEntity(string _time, LogLevel _level, string _location, string _message)
        {
            time = _time;
            level = _level;
            location = _location;
            message = _message;
        }

        public LogEntity() { }
    }
}
