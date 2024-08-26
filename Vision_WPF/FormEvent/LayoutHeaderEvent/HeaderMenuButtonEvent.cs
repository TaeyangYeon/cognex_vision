using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FormEvent
{
    public class HeaderMenuButtonEvent : IButtonEvent
    {
        private static readonly SolidColorBrush selectedColor = new SolidColorBrush(Color.FromRgb(103, 58, 183));
        private static readonly SolidColorBrush NotSelectedColor = new SolidColorBrush(Color.FromRgb(250, 250, 250));

        public void isSelected(Button button) => button.Foreground = selectedColor;

        public void isNotSelected(Button button) => button.Foreground = NotSelectedColor;

        public void setButtonDsn(string buttonName, Dictionary<string, Button> buttons)
        {
            foreach (var btn in buttons)
            {
                if (btn.Key == buttonName)
                    isSelected(btn.Value);
                else
                    isNotSelected(btn.Value);
            }
        }
    }
}
