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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using FormEvent;
using MaterialDesignThemes.Wpf;
using Log;

namespace UI
{
    /// <summary>
    /// Layout_Header.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Layout_Header : UserControl
    {
        private IHeaderEvent headerEvent;

        private MainWindow main;

        private Component_System system;
        private Component_PromptPW promptPW;

        private bool isLockClicked = false;
        private bool isSystemClicked = false;
        private bool isLocked;
        private bool isSystemComponentVisible = false;

        private readonly Logger logger = Logger.instance();

        public Layout_Header(MainWindow _main)
        {
            InitializeComponent();

            this.main = _main;

            headerEvent = new HeaderEventConfig().iHeaderEvent_ver2();

            this.Loaded += layoutHeaderLoad;
        }

        private void layoutHeaderLoad(object sender, RoutedEventArgs e)
        {
            isLocked = true;

            this.VerticalAlignment = VerticalAlignment.Top;
        }

        public void selectMenu(string bodyName)
        {
            headerEvent.selectMenu(bodyName, this);
        }

        private async void mainButtonClick(object sender, RoutedEventArgs e) => await main.setWindow("Main");
        private async void patternButtonClick(object sender, RoutedEventArgs e) => await main.setWindow("Pattern");
        private async void measureButtonClick(object sender, RoutedEventArgs e) => await main.setWindow("Measure");
        private async void logButtonClick(object sender, RoutedEventArgs e) => await main.setWindow("Log");
        private async void interfaceButtonClick(object sender, RoutedEventArgs e) => await main.setWindow("Interface");

        private async Task handleButtonClick(string bodyName)
        {
            await main.setWindow(bodyName);
        }

        private void systemButtonClick(object sender, RoutedEventArgs e)
        {
            toggleSystemComponent();
        }

        private void lockButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isLocked)
            {
                isLocked = true;
                lockIcon.Kind = PackIconKind.Lock;
                headerEvent.selectedButton(false, lock_button);
            }
            else
            {
                isLockClicked = true;
                showPasswordPrompt();
            }
        }

        public void toggleSystemComponent()
        {
            if (!isSystemComponentVisible)
            {
                if (!isLocked) showSystemComponent();
                else
                {
                    isSystemClicked = true;
                    showPasswordPrompt();
                }
            }
            else hideSystemComponent();
        }

        private void showSystemComponent()
        {
            system = new Component_System();

            main.disableWindow();
            isSystemComponentVisible = true;
            system.KeyDown += closeSystemComponent;
            system.exitButton.Click += systemComponentExitButtonClick;
            system.Topmost = true;
            system.showUseFadeIn(0.5);
        }

        private void showPasswordPrompt()
        {
            promptPW = new Component_PromptPW();

            main.disableWindow();
            promptPW.passwordChecked += passwordChecked;
            promptPW.KeyDown += closePromptPWComponent;
            promptPW.exitButton.Click += prompptPWComponentExitButtonClick;
            promptPW.Topmost = true;
            promptPW.showUseFadeIn(0.5);
        }

        private void passwordChecked(object sender, bool isCorrect)
        {
            if (isCorrect)
            {
                hidePromptComponent();

                if (isSystemClicked)
                {
                    showSystemComponent();
                    headerEvent.selectedButton(true, system_button);
                    isSystemClicked = false;
                }

                if (isLockClicked)
                {
                    isLocked = false;
                    lockIcon.Kind = PackIconKind.UnlockedVariantOutline;
                    headerEvent.selectedButton(true, lock_button);
                    isLockClicked = false;
                }

            }
            else
            {
                Component_MessageBox.show("잘못된 비밀번호입니다.", Window.GetWindow(this));
                promptPW.clearPasswordBoxes();
                promptPW.Focus();
            }
        }
        
        private void closeSystemComponent(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            
            hideSystemComponent();
        }

        private void systemComponentExitButtonClick(object sender, EventArgs e)
        {
            hideSystemComponent();
        }

        private void closePromptPWComponent(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;

            isSystemClicked = false;
            isLockClicked = false;

            hidePromptComponent();
        }

        private void prompptPWComponentExitButtonClick(object sender, EventArgs e)
        {
            isSystemClicked = false;
            isLockClicked = false;

            hidePromptComponent();
        }

        private void hidePromptComponent()
        {
            promptPW.hideUseFadeOut(0.5);
            main.enableWindow();
        }

        public void hideSystemComponent()
        {
            system.hideUseFadeOut(0.5);
            isSystemComponentVisible = false;
            headerEvent.selectedButton(false, system_button);

            main.enableWindow();
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            main.exitApplication();
            logger.pushLog(LogLevel.TRACE, "Application Exit!");
            Application.Current.Shutdown();
        }
    }
}
