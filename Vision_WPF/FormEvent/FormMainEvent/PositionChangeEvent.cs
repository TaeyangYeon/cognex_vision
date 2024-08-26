using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FormEvent
{
    public class PositionChangeEvent : IPositionChangeEvent
    {
        private static readonly SolidColorBrush SelectedColor = new SolidColorBrush(Color.FromRgb(103, 58, 183));
        private static readonly SolidColorBrush NotSelectedColor = new SolidColorBrush(Color.FromRgb(250, 250, 250));

        public void isSelected(Button button) => button.Foreground = SelectedColor;

        public void isNotSelected(Button button) => button.Foreground = NotSelectedColor;

        public void setButtonDsn(string buttonName, Dictionary<string, Button> buttons)
        {
            foreach (var btn in buttons)
            {
                if (btn.Key == buttonName) isSelected(btn.Value);
                else isNotSelected(btn.Value);
            }
        }

        public void setListBox(string position, Dictionary<string, ListBox> lists)
        {
            foreach (var list in lists)
            {
                if (list.Key == position) list.Value.Visibility = Visibility.Visible;
                else list.Value.Visibility = Visibility.Collapsed;
            }
        }

        public void changePosition(string item, Dictionary<string, Button> buttons, Dictionary<string, ListBox> lists)
        {
            setButtonDsn(item, buttons);
            setListBox(item, lists);
        }
    }
}
