using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteShutdown.Core;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Server.Core
{
    public class SettingVM : EntityObject
    {
        #region 变量

        static SettingVM _instance = new SettingVM();

        const string SUB_NAME = "Software\\RemoteShutdown";

        const string BOOT_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        #endregion

        #region 属性

        public static SettingVM Instance { get { return _instance; } }

        public bool First
        {
            get { return null == RWReg.GetValue(SUB_NAME, "Server_First", null); }
            set { RWReg.SetValue(SUB_NAME, "Server_First", "Is not first run."); }
        }

        public bool Boot
        {
            get 
            {
                var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Server", string.Empty);
                return !string.IsNullOrEmpty(boot) &&
                    string.Compare(System.Windows.Forms.Application.ExecutablePath,
                        boot, true) == 0;
            }
            set
            {
                if (value)
                {
                    RWReg.SetValue(BOOT_NAME, "RemoteShutdown_Server", System.Windows.Forms.Application.ExecutablePath);
                }
                else
                {
                    RWReg.RemoveKey(BOOT_NAME, "RemoteShutdown_Server");
                }
                RaisePropertyChanged("Boot");
            }
        }

        public string Language
        {
            get
            {
                return ResourcesHelper.ConvertLanguage(
                    RWReg.GetValue(SUB_NAME, "Server_Language",
                        System.Globalization.CultureInfo
                        .InstalledUICulture.EnglishName).ToString());
            }
            set
            {
                RWReg.SetValue(SUB_NAME, "Server_Language", value);
                ResourcesHelper.SetLanguage(value);
                RaisePropertyChanged("Language");
            }
        }

        public IEnumerable<object> Languages
        {
            get
            {
                return ResourcesHelper.GetLanguages();
            }
        }

        #endregion

        #region 构造方法

        private SettingVM() { }

        #endregion
    }
}
