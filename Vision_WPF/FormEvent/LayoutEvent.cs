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
    public class LayoutEvent : ILayoutEvent
    {
        public LayoutEvent() { }

        public void setHeader(UserControl header)
        {
            header.VerticalAlignment = VerticalAlignment.Top;
        }

        public void setBody(Window body)
        {
            body.VerticalAlignment = VerticalAlignment.Bottom;
        }

        public async Task changeBody(string bodyName, Form_Main main, Dictionary<string, Window> bodies)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => UpdateBodyVisibility(bodyName, bodies));

            });
        }

        private void UpdateBodyVisibility(string bodyName, Dictionary<string, Window> bodies)
        {
            foreach (var body in bodies)
            {
                // body.Value.Visibility = body.Key == bodyName ? Visibility.Visible : Visibility.Collapsed;
                if (body.Key == bodyName)
                {
                    body.Value.Show();
                    body.Value.Activate();
                }
                else
                {
                    body.Value.Hide();
                }
            }
        }
    }
}
