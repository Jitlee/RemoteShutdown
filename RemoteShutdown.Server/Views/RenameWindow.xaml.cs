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
    /// RenameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RenameWindow : Window
    {
        public RenameWindow()
        {
            InitializeComponent();
            this.Loaded += RenameWindow_Loaded;
        }

        private void RenameWindow_Loaded(object sender, EventArgs e)
        {
            this.HostNameTextBox.SelectAll();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void HostNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = HostNameTextBox.Text.Length > 0;
        }
    }
}
