using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UI;

namespace FormEvent
{
    public class HeaderEventConfig
    {
        public IButtonEvent iButtonEvent()
        {
            return new HeaderMenuButtonEvent();
        }

        public IHeaderEvent iHeaderEvent_ver2()
        {
            return new HeaderEvent(new LayoutEventConfig().iLayoutEvent(), iButtonEvent());
        }
    }
}
