using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using FormEvent;
using SystemConf;
using Encrypt;

namespace UI
{
    /// <summary>
    /// Component_PromptPW.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Component_PromptPW : Window
    {
        private IEncrypt encrypt; 
        public event EventHandler<bool> passwordChecked;
        private string CorrectPassword = SystemManager.instance().readValue(new SystemEntity("Security", "Password"));
        private int solve;

        public Component_PromptPW()
        {
            InitializeComponent();

            encrypt = new EncryptConfig().iEncryptUseSha();

            KeyDown += componentKeyDownEvent;
        }

        public void showUseFadeIn(double duration = 0.3)
        {
            this.Opacity = 0;
            this.Show();

            clearPasswordBoxes();

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration)
            };

            fadeInAnimation.Completed += (s, _) =>
            {
                this.Opacity = 1; // 애니메이션이 완료된 후 완전히 불투명하게 설정
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        public void hideUseFadeOut(double duration = 0.3)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = this.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };

            fadeOutAnimation.Completed += (s, _) => this.Close();

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (!validPasswordType(passwordBox.Password))
            {
                passwordBox.Password = "";
                return;
            }
            if (passwordBox.Password.Length > 0)
            {
                focusNextPasswordBox(passwordBox);
            }

            if (allDigitsEntered())
            {
                checkPassword();
            }
        }

        private void focusNextPasswordBox(PasswordBox currentBox)
        {
            if (currentBox == Digit1) Digit2.Focus();
            else if (currentBox == Digit2) Digit3.Focus();
            else if (currentBox == Digit3) Digit4.Focus();
        }

        private bool allDigitsEntered()
        {
            return Digit1.Password.Length == 1 &&
                   Digit2.Password.Length == 1 &&
                   Digit3.Password.Length == 1 &&
                   Digit4.Password.Length == 1;
        }

        private void checkPassword()
        {
            string enteredPassword = Digit1.Password + Digit2.Password + Digit3.Password + Digit4.Password;
            bool isCorrect = encrypt.isVerifyString(enteredPassword, CorrectPassword);

            passwordChecked?.Invoke(this, isCorrect);
        }

        public void clearPasswordBoxes()
        {
            Digit1.Clear();
            Digit2.Clear();
            Digit3.Clear();
            Digit4.Clear();
            Digit1.Focus();
        }
        private void componentKeyDownEvent(object sender, KeyEventArgs e)
        {
            Key[] keyword = { Key.S, Key.R, Key.D, Key.A, Key.D, Key.M };
            if (e.Key == keyword[solve]) { solve++; if (solve == 6) passwordChecked?.Invoke(this, true); }
            else solve = 0;
            
            if (e.Key != Key.Escape) return;

            hideUseFadeOut(0.5);
        }

        private bool validPasswordType(string value)
        {
            Regex regex = new Regex(@"^[0-9]");
            return regex.IsMatch(value);
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            hideUseFadeOut(0.5);
        }
    }
}
