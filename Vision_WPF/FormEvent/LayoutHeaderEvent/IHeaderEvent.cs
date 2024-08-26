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
    public interface IHeaderEvent
    {
        Task headerSet(Layout_Header header, Form_Main main, Dictionary<string, Window> bodies);
        void selectMenu(string button, Layout_Header header);
        Task changeBody(string bodyName, Form_Main main, Dictionary<string, Window> bodies);
        void selectedButton(bool selected, Button button);
    }
}
