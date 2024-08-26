using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Threading.Tasks;
using Log;

namespace SystemConf
{
    public class SystemManager
    {
        private static SystemManager systemManager;
        private static readonly object _lock = new object();
        private string filePath;

        private readonly Logger logger = Logger.instance();

        private SystemManager()
        {
            FileInfo basePath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
            filePath = basePath.Directory.FullName + SystemManagerConstValue.SYSTEM_INI_PATH_;

            if (!File.Exists(filePath))
            {
                logger.pushLog(LogLevel.ERROR, $"{filePath} / File is not exitst.");
                throw new FileNotFoundException();
            }
        }

        public static SystemManager instance()
        {
            if (systemManager == null)
            {
                lock (_lock)
                {
                    if (systemManager == null)
                    {
                        systemManager = new SystemManager();
                    }
                }
            }
            return systemManager;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public void writeValue(SystemEntity option)
        {
            try
            {
                checkQueryParameter(option.section, option.key);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            WritePrivateProfileString(option.section, option.key, option.value, filePath);
        }

        public void writeValues(ListDictionary options)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    foreach (DictionaryEntry option in options)
                    {
                        writeValue(option.Value as SystemEntity);
                    }
                    scope.Complete();
                }
                catch (Exception e)
                {
                    logger.pushLog(LogLevel.ERROR, "System.ini 파일 작성 실패");
                    logger.pushLog(LogLevel.ERROR, e.Message);
                    throw e;
                }
            }
        }

        public string readValue(SystemEntity option)
        {
            StringBuilder temp = new StringBuilder(255);
            try
            {
                int i = GetPrivateProfileString(option.section, option.key, "", temp, 255, filePath);
                return temp.ToString();
            }
            catch (Exception e)
            {
                logger.pushLog(LogLevel.ERROR, "System.ini 조회 실패.");
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }
        }
        
        public Dictionary<string, SystemEntity> readAllValues()
        {
            var options = new Dictionary<string, SystemEntity>();
            string currentSection = "";

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("[") && line.EndsWith("]")) currentSection = line.Substring(1, line.Length - 2);
                else
                {
                    string[] content = line.Split('=');
                    options.Add(content[0], new SystemEntity(currentSection, content[0], content[1]));
                }

            }

            return options;
            
        }

        public void checkQueryParameter(string section, string key)
        {
            if (string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(key))
            {
                logger.pushLog(LogLevel.ERROR, "System QueryParameter [section] or [key] is Null or Empty.");
                throw new ArgumentException();
            }
        }
    }
}
