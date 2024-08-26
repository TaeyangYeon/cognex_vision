using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Cognex.VisionPro.Display;
using Cognex.VisionPro;
using PLC;
using FormEvent;
using Log;

namespace UI
{
    /// <summary>
    /// Form_Main.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Form_Main : Window
    {
        private Layout_Header header;

        // private IMainEvent[] mainEvent;
        private IMainEvent_2[] mainEvent;
        private MainWindow main;
        private Component_Loading loading;
        private Component_PromptPW promptPW;
        private Random ranNum;
        private bool isClickAble;

        private WindowsFormsHost host1;
        private WindowsFormsHost host2;
        private WindowsFormsHost host3;
        private WindowsFormsHost host4;
        private CogDisplay display1;
        private CogDisplay display2;
        private CogDisplay display3;
        private CogDisplay display4;
        private CogDisplay[] displays;

        private readonly Logger logger = Logger.instance();
        private List<LogEntity> logList;
        private string logListLevel;

        private string currentDate;

        public Form_Main(MainWindow _main)
        {
            InitializeComponent();

            this.main = _main;

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Width = 1920;
            this.Height = 1080;
            this.Left = 0;
            this.Top = 0;

            header = new Layout_Header(_main);
            headerContent.Content = header;
            
            logger.onLogAdded += addSystemLog;

            currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            this.Loaded += formMainLoad;
        }

        public void formMainLoad(object sender, RoutedEventArgs e)
        {
            header.selectMenu("Main");

            header.connectButton.Click += connectToServer;
            header.modelNameButton.MouseDoubleClick += getModelName;

            loading = new Component_Loading();

            logList = logger.getLogs(DateTime.Now.ToString("yyyy-MM-dd"));
            logListLevel = "ALL";
            loadSystemLog();

            /*
            mainEvent = new IMainEvent[]
            {
                new MainEventConfig().iMainEvent(5000),
                new MainEventConfig().iMainEvent(5001, display1, 0),
                new MainEventConfig().iMainEvent(5002, display2, 1),
                new MainEventConfig().iMainEvent(5003, display3, 2),
                new MainEventConfig().iMainEvent(5004, display4, 3)
            };
            */
            mainEvent = new IMainEvent_2[]
            {
                new MainEventConfig().iMainEvent_2(5000),
                new MainEventConfig().iMainEvent_2(5001),
                new MainEventConfig().iMainEvent_2(5002),
                new MainEventConfig().iMainEvent_2(5003),
                new MainEventConfig().iMainEvent_2(5004)
            };

            ranNum = new Random();

            isClickAble = true;

            mainEvent[0].setMainForm(this);

            setDisplay(displayGrid1, out host1, out display1);
            setDisplay(displayGrid2, out host2, out display2);
            setDisplay(displayGrid3, out host3, out display3);
            setDisplay(displayGrid4, out host4, out display4);

            displays = new CogDisplay[]
            {
                display1, display2, display3, display4
            };

            
        }
        
        private void setDisplay(Grid grid, out WindowsFormsHost host, out CogDisplay display)
        {
            host = new WindowsFormsHost();
            display = new CogDisplay();

            host.Width = 640;
            host.Height = 480;
            host.HorizontalAlignment = HorizontalAlignment.Left;
            host.VerticalAlignment = VerticalAlignment.Top;
            host.Child = display;

            grid.Children.Add(host);

            display.Width = 640;
            display.Height = 480;
            display.BackColor = System.Drawing.Color.Black;
            display.HorizontalScrollBar = false;
            display.VerticalScrollBar = false;
            display.AutoFit = true;
            display.ColorMapLowerClipColor = System.Drawing.Color.Black;
            display.ColorMapLowerRoiLimit = 0D;
            display.ColorMapPredefined = CogDisplayColorMapPredefinedConstants.None;
            display.ColorMapUpperClipColor = System.Drawing.Color.Black;
            display.ColorMapUpperRoiLimit = 1D;
            display.DoubleTapZoomCycleLength = 2;
            display.DoubleTapZoomSensitivity = 2.5D;
            display.MouseWheelMode = CogDisplayMouseWheelModeConstants.Zoom1;
            display.MouseWheelSensitivity = 1D;
            
            Panel.SetZIndex(host, 1);
        }
        

        private void positionButton1Click(object sender, EventArgs e) { if (isClickAble) mainEvent[0].selectPosition("#1", this); }
        private void positionButton2Click(object sender, EventArgs e) { if (isClickAble) mainEvent[0].selectPosition("#2", this); }
        private void positionButton3Click(object sender, EventArgs e) { if (isClickAble) mainEvent[0].selectPosition("#3", this); }
        private void positionButton4Click(object sender, EventArgs e) { if (isClickAble) mainEvent[0].selectPosition("#4", this); }

        public async void connectToServer(object sender, RoutedEventArgs e)
        {
            showLoading("Connecting");

            await Task.Delay(100);

            try
            {
                foreach (var mEvent in mainEvent) await mEvent.socketConnect();
            }
            catch (SocketConnectFailException ex)
            {
                Component_MessageBox.show(ex.Message, Window.GetWindow(this));
            }
            catch (SocketException)
            {
                Component_MessageBox.show($"소캣 생성 실패 / PLC 상태 확인 필요", GetWindow(this));
            }

            await Task.Delay(1000);

            hideLoading();
        }

        public async void getModelName(object sender, RoutedEventArgs e)
        {
            try
            {
                header.modelName.Content = await mainEvent[0].getModelName("100");
            }
            catch (Exception ex)
            {
                Component_MessageBox.show($"연결된 서버가 없습니다.", GetWindow(this));
            }
        }

        private async void connectToCamera(object sender, RoutedEventArgs e)
        {
            showLoading("Init Camera");

            try
            {
                /*
                mainEvent[1].initializeCamera();
                mainEvent[2].initializeCamera();
                mainEvent[3].initializeCamera();
                mainEvent[4].initializeCamera();
                */
                await mainEvent[1].initializeCamera();
            }
            catch(Exception ex)
            {
                Component_MessageBox.show("카메라 초기화를 실패하였습니다. \n카메라 연결 확인이 필요합니다.", GetWindow(this));
            }

            await Task.Delay(1000);

            hideLoading();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async void checkedChangedDisplay1(object sender, RoutedEventArgs e) => await toggleLiveMode(sender as ToggleButton, 0);
        private async void checkedChangedDisplay2(object sender, RoutedEventArgs e) => await toggleLiveMode(sender as ToggleButton, 1);
        private async void checkedChangedDisplay3(object sender, RoutedEventArgs e) => await toggleLiveMode(sender as ToggleButton, 2);
        private async void checkedChangedDisplay4(object sender, RoutedEventArgs e) => await toggleLiveMode(sender as ToggleButton, 3);

        private bool isUpdating = false;
        private async Task toggleLiveMode(ToggleButton sender, int index)
        {
            if (isUpdating) return;

            isUpdating = true;
            try
            {
                /*
                mainEvent[index].toggleLiveMode(sender.IsChecked == true);

                if (sender.IsChecked == false) displays[index].Image = mainEvent[index].grabImage();
                */
                mainEvent[1].toggleLiveMode(index, sender.IsChecked == true, displays[index]);

                if (sender.IsChecked == false) displays[index].Image = await mainEvent[1].grabImage(index, displays[index]);
            }
            catch (Exception)
            {
                sender.IsChecked = false;
                Component_MessageBox.show("카메라 연결 확인이 필요합니다.", GetWindow(this));
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void changeLogType(object sender, RoutedEventArgs e)
        {
            if (logTitle.Content.Equals("System Log"))
            {
                logTitle.Content = "Align Log";
                mainEvent[0].selectPosition("#1", this);
                logLevel.Visibility = Visibility.Collapsed;
                isClickAble = true;
            }
            else
            {
                showPasswordPrompt();
            }
        }

        private void changeLogTypeSystem()
        {
            logTitle.Content = "System Log";
            mainEvent[0].selectPosition("None", this);
            logLevel.Visibility = Visibility.Visible;
            system_log_list.Visibility = Visibility.Visible;
            logLevel.SelectedIndex = (logListLevel == "ALL") ? 0 : (logListLevel == "TRACE") ? 1 : (logListLevel == "DEBUG") ? 2 : (logListLevel == "INFO") ? 3 : (logListLevel == "WARN") ? 4 : 5;
            loadSystemLog();
            isClickAble = false;
            
        }

        public void showLoading(string content)
        {
            loading.Title.Text = content;
            loading.Show();
            loading.Topmost = true;
        }

        public void hideLoading()
        {
            loading.Hide();
        }

        private async void alignCycleTestClick(object sender, RoutedEventArgs e)
        {
            var results = new List<int>();
            var AlignValues = new List<(int x, int y)>();
            for (int i = 0; i < 4; i++)
            {
                results.Add(getRanNum(1, 2));
                AlignValues.Add((getRanNum(1, 100), getRanNum(1, 100)));
            }
            try
            {
                var task1 = mainEvent[1].testCycle(1, this, results[0], AlignValues[0]);
                var task2 = mainEvent[2].testCycle(2, this, results[1], AlignValues[1]);
                var task3 = mainEvent[3].testCycle(3, this, results[2], AlignValues[2]);
                var task4 = mainEvent[4].testCycle(4, this, results[3], AlignValues[3]);

                await Task.WhenAll(task1, task2, task3, task4);
            }
            catch (NotChangedPlcSignal ex)
            {
                Component_MessageBox.show($"{ex.Message}", GetWindow(this));
            }
            catch (Exception)
            {
                Component_MessageBox.show($"PLC 통신 오류 발생", GetWindow(this));
            }
        }

        private int getRanNum(int min, int max)
        {
            return ranNum.Next(min, max + 1);
        }

        private void showPasswordPrompt()
        {
            promptPW = new Component_PromptPW();

            main.disableWindow();
            promptPW.passwordChecked += passwordChecked;
            promptPW.KeyDown += closePromptPwComponent;
            promptPW.exitButton.Click += prompptPwComponentExitButtonClick;  
            promptPW.Topmost = true;
            promptPW.showUseFadeIn(0.5);
        }

        private void passwordChecked(object sender, bool isCorrect)
        {
            if (isCorrect)
            {
                promptPW.hideUseFadeOut(0.5);

                changeLogTypeSystem();

                main.enableWindow();
            }
            else
            {
                Component_MessageBox.show("잘못된 비밀번호입니다.", Window.GetWindow(this));
                promptPW.clearPasswordBoxes();
                promptPW.Focus();
            }
        }

        private void closePromptPwComponent(object sender, KeyEventArgs e)
        {
            main.enableWindow();
        }

        private void prompptPwComponentExitButtonClick(object sender, EventArgs e)
        {
            main.enableWindow();
        }

        private void loadSystemLog()
        {
            this.Dispatcher.Invoke(() =>
            {
                system_log_list.Items.Clear();

                var list = logList;
                
                if (Enum.IsDefined(typeof(LogLevel), logListLevel)) 
                {
                    var level = (LogLevel)Enum.Parse(typeof(LogLevel), logListLevel);
                    list = logList.Where(log => log.level == level).ToList();
                }

                foreach (var log in list)
                {
                    writeSystemLogListRow(log);
                }
            });
        }

        private void addSystemLog(object sender, LogEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                if (!currentDate.Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    system_log_list.Items.Clear();
                    currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                    loadSystemLog();
                }

                if (logListLevel.Equals("ALL")) writeSystemLogListRow(e.log);
                else if (!logListLevel.Equals("ALL") && e.log.level == (LogLevel)Enum.Parse(typeof(LogLevel), logListLevel)) writeSystemLogListRow(e.log); 

                logList.Add(e.log);
            });
        }

        private void writeSystemLogListRow(LogEntity _log)
        {
            string[] log = _log.ToString().Split(',');
            system_log_list.Items.Insert(0, $"{log[0]} | {log[1]} | {log[2]} | {log[3]}");
        }

        private void updateSystemLogList(LogLevel level)
        {
            var list = logList.Where(log => log.level == level);

            system_log_list.Items.Clear();
            foreach (var li in list) writeSystemLogListRow(li);
        }

        private void selectSystemLogLevel(object sender, SelectionChangedEventArgs e)
        {
            if (logLevel.SelectedItem is ComboBoxItem selectedItem)
            {
                logListLevel = selectedItem.Content as string;
                loadSystemLog();
            }
        }

        public void closeMainBody()
        {
            foreach (var mEvent in mainEvent) mEvent.Dispose();
        }

        private void Position_button1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainEvent[1].loadImage(displays[0]);
        }
    }
}
