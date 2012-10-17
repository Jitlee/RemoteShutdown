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

namespace RemoteShutdown.Server.Views
{
    /// <summary>
    /// SendMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendMessageWindow : Window
    {
        public SendMessageWindow()
        {
            InitializeComponent();
            this.Loaded += SendMessageWindow_Loaded;
        }

        private void SendMessageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MessageTextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = MessageTextBox.Text.Length > 0;
        }
    }
}
