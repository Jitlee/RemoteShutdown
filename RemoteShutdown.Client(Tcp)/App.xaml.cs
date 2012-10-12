using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic.ApplicationServices;
using RemoteShutdown.Client.Views;
using RemoteShutdown.Client.Core;

namespace RemoteShutdown.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Text = "远程关机客户端";
            _notifyIcon.Icon = RemoteShutdown.Client.Properties.Resources.app;
            _notifyIcon.Visible = true;

            System.Windows.Forms.MenuItem setting = new System.Windows.Forms.MenuItem("设置");
            setting.Click += Setting_Click;

            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += Exit_Click;

            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { setting, exit };
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            if (SettingVM.Instance.First) // 设置开机运行
            {
                SettingVM.Instance.Boot = true;
                SettingVM.Instance.First = false;
                ShowSettingWindow();
            }

            base.OnStartup(e);
        }

        SettingWindow _settingWindow;
        private void Setting_Click(object sender, EventArgs e)
        {
            ShowSettingWindow();
        }

        private void ShowSettingWindow()
        {
            Setting();
        }

        public void Setting()
        {
            if (null == _settingWindow)
            {
                _settingWindow = new SettingWindow();
                _settingWindow.Closed += (obj, args) => { _settingWindow = null; };
                _settingWindow.Show();
                _settingWindow.Activate();
                _settingWindow.Focus();
            }
            else
            {
                if (_settingWindow.WindowState == WindowState.Minimized)
                {
                    _settingWindow.WindowState = WindowState.Normal;
                }
                _settingWindow.Activate();
                _settingWindow.Focus();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (null != _notifyIcon)
            {
                _notifyIcon.Dispose();
            }
            base.OnExit(e);
        }

        public class EntryPoint
        {
            [STAThread]
            public static void Main(string[] args)
            {
                SingleInstanceManager manager = new SingleInstanceManager();
                manager.Run(args);
            }
        }

        // Using VB bits to detect single instances and process accordingly:
        //  * OnStartup is fired when the first instance loads
        //  * OnStartupNextInstance is fired when the application is re-run again
        //    NOTE: it is redirected to this instance thanks to IsSingleInstance
        public class SingleInstanceManager : WindowsFormsApplicationBase
        {
            App app;

            public SingleInstanceManager()
            {
                this.IsSingleInstance = true;
            }

            protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
            {
                // First time app is launched
                app = new App();
                app.InitializeComponent();

                if (e.CommandLine.Contains("/s", StringComparer.CurrentCultureIgnoreCase))
                {
                    app.Setting();
                }
                app.Run();
                return false;
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                // Subsequent launches
                base.OnStartupNextInstance(eventArgs);

                if (eventArgs.CommandLine.Contains("/s", StringComparer.CurrentCultureIgnoreCase))
                {
                    app.Setting();
                }
            }
        }
    }
}
