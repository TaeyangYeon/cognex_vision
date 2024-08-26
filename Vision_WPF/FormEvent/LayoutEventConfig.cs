using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI;

namespace FormEvent
{
    public class LayoutEventConfig
    {
        public ILayoutEvent iLayoutEvent()
        {
            return new LayoutEvent();
        }
    }
}
