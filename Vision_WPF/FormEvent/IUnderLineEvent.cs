using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FormEvent
{
    public interface IUnderLineEvent
    {
        void isSelected(Panel line);
        void isNotSelected(Panel line);
        void setUnderLineDsn(string item, Dictionary<string, Panel> lines);
    }
}
