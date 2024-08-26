using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Log;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Form_Main main;
        private Form_Pattern pattern;
        private Form_Measure measure;
        private Form_Log log;
        private Form_Interface vinterface;

        private Dictionary<string, Window> windows;

        private readonly Logger logger = Logger.instance();

        public MainWindow()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();

            Loaded += mainWindowLoad;
        }

        private void mainWindowLoad(object sender, RoutedEventArgs e)
        {
            logger.pushLog(LogLevel.TRACE, "Application Start!");

            this.WindowState = WindowState.Maximized;

            this.Left = 0;
            this.Top = 0;

            main = new Form_Main(this);
            pattern = new Form_Pattern(this);
            pattern.Hide();
            measure = new Form_Measure(this);
            measure.Hide();
            log = new Form_Log(this);
            log.Hide();
            vinterface = new Form_Interface(this);
            vinterface.Hide();



            windows = new Dictionary<string, Window>
            {
                {"Main", main },
                {"Pattern", pattern },
                {"Measure", measure },
                {"Log", log },
                {"Interface", vinterface }
            };

            main.Show();

            this.Hide();
        }

        public void disableWindow()
        {
            foreach (var window in windows) window.Value.IsEnabled = false;
        }

        public void enableWindow()
        {
            foreach (var window in windows) window.Value.IsEnabled = true;
        }

        public async Task setWindow(string window)
        {
            var currentWindow = windows.FirstOrDefault(w => w.Value.IsVisible).Value;
            var nextWindow = windows[window];

            if (currentWindow == nextWindow) return;

            nextWindow.Opacity = 0;
            nextWindow.Show();

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.05));
            var fadeIn = new DoubleAnimation(1, 1, TimeSpan.FromSeconds(0));

            if (currentWindow != null)
            {
                currentWindow.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
            nextWindow.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay(50);

            if (currentWindow != null)
            {
                currentWindow.Hide();
            }
        }

        public void exitApplication()
        {
            main.closeMainBody();
        }
    }
}
