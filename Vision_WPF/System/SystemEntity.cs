using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemConf
{
    public class SystemEntity
    {
        public string section { get; set; }
        public string key { get; set; }
        public string value { get; set; }

        public SystemEntity(string _section, string _key)
        {
            this.section = _section;
            this.key = _key;
        }
        public SystemEntity(string _section, string _key, string _value)
        {
            this.section = _section;
            this.key = _key;
            this.value = _value;
        }

        public SystemEntity() { }
        
    }
}
