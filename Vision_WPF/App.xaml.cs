using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI;

namespace Vision_WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var tempWindow = new Component_Loading();

            tempWindow.Topmost = true;
            tempWindow.Show();

            await Task.Delay(500);
            
            MainWindow mainWindow = new MainWindow();

            // UI 스레드에서 메인 창 표시
            Dispatcher.Invoke(() =>
            {
                mainWindow.Visibility = Visibility.Visible;
            });

            await Task.Delay(1000);

            tempWindow.Close();

        }
    }
}
