using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Media.Effects;
using MaterialDesignThemes.Wpf;
using FormEvent;
using SystemConf;

namespace UI
{
    /// <summary>
    /// Form_Pattern.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Component_System : Window
    {
        private ISystemEvent systemEvent;

        private ListDictionary changedOptionList;

        private Timer debounceTimer;
        private string lastSearchText;
        private bool isPasswordChanged;

        private SystemManager system = SystemManager.instance();

        public Component_System()
        {
            InitializeComponent();
            changedOptionList = new ListDictionary();
            
            systemEvent = new SystemEventConfig().iSystemEvent(this, optionsPanel);


            lastSearchText = "";

            changedOptionList.Clear();
        }

        public void showUseFadeIn(double duration = 0.3)
        {
            this.Opacity = 0;
            this.Show();

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

        public void activateComponent() => systemEvent.activeComponent(this);

        public void disactiveComponent() => systemEvent.disactiveComponent(this);
        
        private void searchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            if (searchText == lastSearchText) return;

            lastSearchText = searchText;
            
            debounceTimer?.Dispose();
            
            debounceTimer = new Timer(_ =>
            {
                Dispatcher.Invoke(() => systemEvent.performSearch(searchText));
            }, null, 300, Timeout.Infinite);
        }

        private void modeCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {                
                string value = checkBox.IsChecked == true ? "1" : "0";

                if (changedOptionList.Contains(checkBox.Name)) changedOptionList[checkBox.Name] = new SystemEntity("Mode", checkBox.Name, value);
                else changedOptionList.Add(checkBox.Name, new SystemEntity("Mode", checkBox.Name, value));
            }
        }

        private void modeOptionTextChanged(object sender, TextChangedEventArgs e)
       {
            if (sender is TextBox textBox)
            {
                if (!validValueType(textBox.Text)) { textBox.Text = ""; return; }

                debounceTimer?.Dispose();

                debounceTimer = new Timer(_ =>
                {
                    Dispatcher.Invoke(() => 
                    {
                        if (changedOptionList.Contains(textBox.Name)) changedOptionList[textBox.Name] = new SystemEntity("Mode", textBox.Name, textBox.Text);
                        else changedOptionList.Add(textBox.Name, new SystemEntity("Mode", textBox.Name, textBox.Text));
                    });
                }, null, 300, Timeout.Infinite);
            }
        }

        private void signalOptionTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!validValueType(textBox.Text)) { textBox.Text = ""; return; }

                debounceTimer?.Dispose();

                debounceTimer = new Timer(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (changedOptionList.Contains(textBox.Name)) changedOptionList[textBox.Name] = new SystemEntity("Signal", textBox.Name, textBox.Text); 
                        else Dispatcher.Invoke(() => changedOptionList.Add(textBox.Name, new SystemEntity("Signal", textBox.Name, textBox.Text)));
                    });
                }, null, 300, Timeout.Infinite);
            }
        }

        private void modeOptionSselectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is ComboBoxItem _item)
                {   
                    string item = _item.ToString().Split(' ')[1];

                    if (changedOptionList.Contains(comboBox.Name)) changedOptionList[comboBox.Name] = new SystemEntity("Mode", comboBox.Name, item);
                    else changedOptionList.Add(comboBox.Name, new SystemEntity("Mode", comboBox.Name, item));
                }
            }
        }

        private void systemSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (isPasswordChanged)
            {
                if (Password.Password.Length != 4)
                {
                    Component_MessageBox.show("비밀번호를 네 자리로 설정해야 합니다.", GetWindow(this));
                    return;
                }
            }
            systemEvent.saveOptions(changedOptionList);

            Password.Password = "";
            changedOptionList = new ListDictionary();
            isPasswordChanged = false;
        }

        private void systemLoadButtonClick(object sender, RoutedEventArgs e)
        {
            Password.Password = "";
            systemEvent.initOptionsValue();
        }

        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            
            if (sender is PasswordBox passwordBox)
            {
                if (!validValueType(passwordBox.Password)) { passwordBox.Password = ""; return; }

                debounceTimer?.Dispose();
                
                debounceTimer = new Timer(_ =>
                {
                    Dispatcher.Invoke(() => 
                    {
                        if (changedOptionList.Contains(passwordBox.Name)) changedOptionList[passwordBox.Name] = new SystemEntity("Security", passwordBox.Name, systemEvent.encryptedString(passwordBox.Password));
                        else changedOptionList.Add(passwordBox.Name, new SystemEntity("Security", passwordBox.Name, systemEvent.encryptedString(passwordBox.Password)));
                    });
                }, null, 300, Timeout.Infinite);

                isPasswordChanged = true;
            }
        }

        private bool validValueType(string value)
        {
            Regex regex = new Regex(@"^[0-9]");
            return regex.IsMatch(value);
        }
    }
} 
