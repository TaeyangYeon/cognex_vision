using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SystemConf;

namespace FormEvent
{
    public interface ISystemEvent
    {
        void initOptionsValue();
        string getOptionValueText(string key);
        void saveOptions(ListDictionary options);
        void activeComponent(Window component);
        void disactiveComponent(Window component);
        void performSearch(string searchText);
        string encryptedString(string value);
        bool isVerifyString(string value, string hashString);
    }
}
