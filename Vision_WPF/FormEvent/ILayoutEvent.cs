using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI;

namespace FormEvent
{
    public interface ILayoutEvent
    {
        void setHeader(UserControl header);
        void setBody(Window body);
        Task changeBody(string body, Form_Main main, Dictionary<string, Window> bodies);
    }
}
