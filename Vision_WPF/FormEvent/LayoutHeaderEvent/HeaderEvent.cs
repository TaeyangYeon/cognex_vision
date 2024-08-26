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
    public class HeaderEvent : IHeaderEvent
    {
        private ILayoutEvent layout;
        private IButtonEvent buttonEvent;
        
        public HeaderEvent(ILayoutEvent _layout, IButtonEvent _buttonEvent)
        {
            this.layout = _layout;
            this.buttonEvent = _buttonEvent;
        }

        public async Task headerSet(Layout_Header header, Form_Main main, Dictionary<string, Window> bodies)
        {
            layout.setHeader(header);

            await layout.changeBody("Main", main, bodies);

            selectMenu("Main", header);
        }

        public void selectMenu(string buttonName, Layout_Header header)
        {
            var buttons = new Dictionary<string, Button>
            {
                {"Main", header.main_button },
                {"Pattern", header.pattern_button },
                {"Measure", header.measure_button },
                {"Log", header.log_button },
                {"Interface", header.interface_button }
            };
            buttonEvent.setButtonDsn(buttonName, buttons);
        }

        public void selectedButton(bool selected, Button button)
        {
            if (selected) buttonEvent.isSelected(button);
            else buttonEvent.isNotSelected(button);
        }

        public async Task changeBody(string bodyName, Form_Main main, Dictionary<string, Window> bodies)
        {
            await layout.changeBody(bodyName, main, bodies);
        }
    }
}
