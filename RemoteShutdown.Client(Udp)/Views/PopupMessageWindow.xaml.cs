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
using RemoteShutdown.Core;

namespace RemoteShutdown.Views
{
    /// <summary>
    /// PopupMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupMessageWindow : Window
    {
        //private static double _count = 0d;
        private static List<double> _indexes = new List<double>() { -1d };

        private double _index = 0;

        private PopupMessageWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _indexes.Remove(_index);
        }

        public static void Show(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                var window = new PopupMessageWindow();
                window.Title = string.Format(ResourcesHelper.GetValue("ServerMessageString", "服务器消息[{0:yy-M-d HH:mm:ss}]"), DateTime.Now);
                window.MessageTextBox.Text = message;
                window._index = _indexes.Max() + 1;
                _indexes.Add(window._index);

                var maxVerticalCount = Math.Floor(SystemParameters.FullPrimaryScreenHeight / (window.Height + 10));
                var top = SystemParameters.FullPrimaryScreenHeight - ((window._index) % maxVerticalCount + 1d) * (window.Height + 10);
                var left = SystemParameters.FullPrimaryScreenWidth - (Math.Floor(window._index / maxVerticalCount) + 1d) * window.Width;
                window.Left = left;
                window.Top = top;
                window.Show();
            }));

            //_count++;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
