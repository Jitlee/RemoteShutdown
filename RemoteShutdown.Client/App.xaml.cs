using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic.ApplicationServices;

using RemoteShutdown.Client.Core;
using RemoteShutdown.Client.Views;
using RemoteShutdown.Core;

namespace RemoteShutdown.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.MenuItem _setting = new System.Windows.Forms.MenuItem("设置");
        System.Windows.Forms.MenuItem _exit = new System.Windows.Forms.MenuItem("退出");

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            var language = SettingVM.Instance.Language;
            ResourcesHelper.SetLanguage(language);

            UpdateLanguage();

            SettingVM.Instance.PropertyChanged += Instance_PropertyChanged;

            _notifyIcon.Icon = RemoteShutdown.Client.Properties.Resources.app;
            _notifyIcon.Visible = true;

            _setting.Click += Setting_Click;

            _exit.Click += Exit_Click;

            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { _setting, _exit };
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            _notifyIcon.MouseClick += NotifyIcon_MouseClick;

            if (SettingVM.Instance.First) // 设置开机运行
            {
                SettingVM.Instance.Boot = true;
                SettingVM.Instance.First = false;
                ShowSettingWindow();
            }

            base.OnStartup(e);
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                UpdateLanguage();
            }
        }

        private void UpdateLanguage()
        {
            _notifyIcon.Text = ResourcesHelper.GetValue("TitleString", "教育云交互平台终端远程控制服务端");
            _setting.Text = ResourcesHelper.GetValue("SettingString", "设置");
            _exit.Text = ResourcesHelper.GetValue("ExitString", "退出");
        }

        SettingWindow _settingWindow;
        private void Setting_Click(object sender, EventArgs e)
        {
            ShowSettingWindow();
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (null != MainWindow)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
                MainWindow.Focus();
            }
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
