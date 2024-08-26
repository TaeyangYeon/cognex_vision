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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Log;

namespace UI
{
    /// <summary>
    /// Form_Pattern.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Form_Pattern : Window
    {
        private Layout_Header header;

        private readonly Logger logger = Logger.instance();

        public Form_Pattern(MainWindow main)
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;  // 타이틀 바 제거
            this.ResizeMode = ResizeMode.NoResize;
            this.Width = 1920;  // 원하는 크기로 설정
            this.Height = 1080;
            this.Left = 0;
            this.Top = 0;

            header = new Layout_Header(main);

            headerContent.Content = header;
        }

        private void formPatternLoad(object sender, EventArgs e)
        {
            header.selectMenu("Pattern");
        }
    }
}
