using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Component_MessageBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Component_MessageBox : Window
    {
        public Component_MessageBox(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
            Topmost = true;
            this.KeyDown += escapeComponent;
            this.KeyDown += confirmComponent;
        }

        private void confirmButtonClick(object sender, RoutedEventArgs e) => closeComponent();
        
        private void escapeComponent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) closeComponent();
        }

        private void confirmComponent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) closeComponent();
        }

        private void closeComponent()
        {
            DialogResult = true;
            this.Close();
        }

        public static void show(string message, Window owner = null)
        {
            var messageBox = new Component_MessageBox(message);
            if (owner != null)
            {
                messageBox.Owner = owner;
            }
            messageBox.ShowDialog();
        }
    }
}
