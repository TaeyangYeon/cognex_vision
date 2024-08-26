using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI;

namespace FormEvent
{
    public class SystemEventConfig
    {
        public ISystemEvent iSystemEvent(Component_System system,StackPanel panel)
        {
            return new SystemEvent(system, panel);
        }
    }
}
