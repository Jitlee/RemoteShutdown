using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RemoteShutdown.Views
{
    /// <summary>
    /// PopupMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupMessageWindow : Window
    {
        private static double _count = 0d;
        private PopupMessageWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _count--;
        }

        public static void Show(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                var window = new PopupMessageWindow();
                window.Title = string.Format("服务器消息[{0:yy-M-d HH:mm:ss}]", DateTime.Now);
                window.MessageTextBox.Text = message;

                var maxVerticalCount = Math.Floor(SystemParameters.FullPrimaryScreenHeight / (window.Height + 10));
                var top = SystemParameters.FullPrimaryScreenHeight - (_count % maxVerticalCount + 1d) * (window.Height + 10);
                var left = SystemParameters.FullPrimaryScreenWidth - (Math.Floor(_count / maxVerticalCount) + 1d) * window.Width;
                window.Left = left;
                window.Top = top;
                window.Show();
            }));

            _count++;
        }
    }
}
