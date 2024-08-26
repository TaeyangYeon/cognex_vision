using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FormEvent
{
    public interface IButtonEvent
    {
        void isSelected(Button button);
        void isNotSelected(Button button);
        void setButtonDsn(string buttonName, Dictionary<string, Button> buttons);
    }
}
