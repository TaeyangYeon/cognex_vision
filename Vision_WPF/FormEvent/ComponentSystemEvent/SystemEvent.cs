using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using SystemConf;
using UI;
using Encrypt;

namespace FormEvent
{
    public class SystemEvent : ISystemEvent
    {
        private Component_System systemComponent;

        private IEncrypt encrypt;

        private Dictionary<string, Card> options;
        private Dictionary<string, SystemEntity> systemValues;
        private Color highlightColor = Colors.Purple;
        private Thickness defaultThickness = new Thickness(1);
        private Thickness highlightThickness = new Thickness(2);

        private SystemManager system = SystemManager.instance();

        public SystemEvent(Component_System _systemComponent, StackPanel panel)
        {
            this.systemComponent = _systemComponent;

            encrypt = new EncryptConfig().iEncryptUseSha();

            options = new Dictionary<string, Card>();

            systemValues = system.readAllValues();
            
            InitializeOptionElements(panel);

            initOptionsValue();
        }

        public void activeComponent(Window component) => component.IsEnabled = true;
        public void disactiveComponent(Window component) => component.IsEnabled = false;
        
        private void InitializeOptionElements(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (!(child is Card card)) continue;

                var content = card.Content as StackPanel;
                if (content == null) continue;

                string key = getKeyFromContent(content);
                if (string.IsNullOrEmpty(key)) continue;

                options[key.ToLower()] = card;
            }
        }
        public void initOptionsValue()
        {
            systemComponent.ImageLogSaveMode.IsChecked = isOptionChecked("ImageLogSaveMode");
            systemComponent.MarkAllSearchMode.IsChecked = isOptionChecked("MarkAllSearchMode");
            systemComponent.ResearchMode.IsChecked = isOptionChecked("ResearchMode");
            systemComponent.PatternSearchMode.IsChecked = isOptionChecked("PatternSearchMode");
            systemComponent.ManualVerifyMode.IsChecked = isOptionChecked("ManualVerifyMode");
            systemComponent.NGOrgImageSaveMode.IsChecked = isOptionChecked("NGOrgImageSaveMode");
            systemComponent.ReverseDirection.IsChecked = isOptionChecked("ReverseDirection");
            systemComponent.CircleSearchMode.IsChecked = isOptionChecked("CircleSearchMode");
            systemComponent.ManualCenterMode.IsChecked = isOptionChecked("ManualCenterMode");
            systemComponent.LogSaveDays.Text = getOptionValueText("LogSaveDays");
            systemComponent.CameraSet.SelectedItem = selectedItem(systemComponent.CameraSet, "CameraSet");
            systemComponent.BrightSet.SelectedItem = selectedItem(systemComponent.BrightSet, "BrightSet");

            systemComponent.StartAddressAlign.Text = getOptionValueText("StartAddressAlign");
            systemComponent.VisionOnlineAck.Text = getOptionValueText("VisionOnlineAck");
            systemComponent.OutputSignalAlign.Text = getOptionValueText("OutputSignalAlign");
            systemComponent.PlcOnlineCheck.Text = getOptionValueText("PlcOnlineCheck");
            systemComponent.ModelNumber.Text = getOptionValueText("ModelNumber");
            systemComponent.ModelName.Text = getOptionValueText("ModelName");
            systemComponent.UserVerifyAck.Text = getOptionValueText("UserVerifyAck");
            systemComponent.UserVerifyComplete.Text = getOptionValueText("UserVerifyComplete");
            systemComponent.IPAddress.Text = getOptionValueText("IPAddress");
            systemComponent.Port.Text = getOptionValueText("Port");
            systemComponent.PCOff.Text = getOptionValueText("PCOff");
            systemComponent.IDAddress.Text = getOptionValueText("IDAddress");
            systemComponent.PlugIndex.Text = getOptionValueText("PlugIndex");
            systemComponent.AlignWordX.Text = getOptionValueText("AlignWordX");
            systemComponent.AlignWordY.Text = getOptionValueText("AlignWordY");
            systemComponent.AlignWordA.Text = getOptionValueText("AlignWordA");
            systemComponent.AlignDirectionX.Text = getOptionValueText("AlignDirectionX");
            systemComponent.AlignDirectionY.Text = getOptionValueText("AlignDirectionY");
            systemComponent.AlignDirectionA.Text = getOptionValueText("AlignDirectionA");
            systemComponent.PlcToCalibReady.Text = getOptionValueText("PlcToCalibReady");
            systemComponent.PlcToMoveCompleteX.Text = getOptionValueText("PlcToMoveCompleteX");
            systemComponent.PlcToMoveCompleteY.Text = getOptionValueText("PlcToMoveCompleteY");
            systemComponent.PlcToMoveCompleteA.Text = getOptionValueText("PlcToMoveCompleteA");
            systemComponent.PlcToMoveCompleteC.Text = getOptionValueText("PlcToMoveCompleteC");
            systemComponent.VisionToCalibReady.Text = getOptionValueText("VisionToCalibReady");
            systemComponent.VisionToMoveRequestX.Text = getOptionValueText("VisionToMoveRequestX");
            systemComponent.VisionToMoveRequestY.Text = getOptionValueText("VisionToMoveRequestY");
            systemComponent.VisionToMoveRequestA.Text = getOptionValueText("VisionToMoveRequestA");
            systemComponent.VisionToMoveRequestC.Text = getOptionValueText("VisionToMoveRequestC");
            systemComponent.CalibWordX.Text = getOptionValueText("CalibWordX");
            systemComponent.CalibWordY.Text = getOptionValueText("CalibWordY");
            systemComponent.CalibWordA.Text = getOptionValueText("CalibWordA");
        }

        public string getOptionValueText(string key)
        {
            return systemValues[key].value;
        }
        private bool isOptionChecked(string key) => (systemValues[key].value.Equals("1")) ? true : false;
        private ComboBoxItem selectedItem(ComboBox box, string key)
        {
            return box.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString().Equals(systemValues[key].value));
        }

        public void saveOptions(ListDictionary options)
        {
            system.writeValues(options);

            systemValues = system.readAllValues();
        }

        private void applySettingToCard(Card card, string value)
        {
            var content = card.Content as StackPanel;
            if (content == null) return;

            var checkbox = content.Children.OfType<CheckBox>().FirstOrDefault();
            if (checkbox != null && !string.IsNullOrEmpty(value))
            {
                checkbox.IsChecked = value.Equals("1") ? true : false;
                return;
            }

            var textbox = content.Children.OfType<TextBox>().FirstOrDefault();
            if (textbox != null && !string.IsNullOrEmpty(value))
            {
                textbox.Text = value;
                return;
            }

            var combobox = content.Children.OfType<ComboBox>().FirstOrDefault();
            if (combobox != null && !string.IsNullOrEmpty(value))
            {
                combobox.SelectedItem = combobox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == value);
            }
        }

        private string getKeyFromContent(StackPanel content)
        {
            var checkbox = content.Children.OfType<CheckBox>().FirstOrDefault();

            if (checkbox != null) return checkbox.Content?.ToString();

            var titleTextBlock = content.Children.OfType<TextBlock>().FirstOrDefault();

            if (titleTextBlock != null) return titleTextBlock.Text;

            return null;
        }

        public void performSearch(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) { clearHighlights(); return; }

            var option = options.FirstOrDefault(o => o.Key.Contains(searchText)).Value;

            if (option == null) { clearHighlights(); return; }

            scrollToElement(option);
            highlightElement(option);
        } 

        private void scrollToElement(FrameworkElement element)
        {
            if (element != null) element.BringIntoView();
        }

        private void highlightElement(Card card)
        {
            clearHighlights();

            if (card != null)
            {
                card.BorderBrush = new SolidColorBrush(highlightColor);
                card.BorderThickness = highlightThickness;
                card.Effect = new DropShadowEffect
                {
                    Color = highlightColor,
                    ShadowDepth = 0,
                    BlurRadius = 10,
                    Opacity = 0.5
                };
            }
        }

        private void clearHighlights()
        {
            foreach (var card in options.Values)
            {
                card.BorderBrush = new SolidColorBrush(Colors.Transparent);
                card.BorderThickness = defaultThickness;
                card.Effect = null;
            }
        }

        public string encryptedString(string value) => encrypt.getHashString(value);

        public bool isVerifyString(string value, string hashString) => encrypt.isVerifyString(value, hashString);

    }
}
