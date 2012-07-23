using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteShutdown.Core;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Client.Core
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
            get { return null == RWReg.GetValue(SUB_NAME, "Udp_Client_First", null); }
            set { RWReg.SetValue(SUB_NAME, "Udp_Client_First", "Is not first run."); }
        }

        public bool Boot
        {
            get 
            {
                var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Udp_Client", string.Empty);
                return !string.IsNullOrEmpty(boot) &&
                    string.Compare(System.Windows.Forms.Application.ExecutablePath,
                        boot, true) == 0;
            }
            set
            {
                if (value)
                {
                    RWReg.SetValue(BOOT_NAME, "RemoteShutdown_Udp_Client", System.Windows.Forms.Application.ExecutablePath);
                }
                else
                {
                    RWReg.RemoveKey(BOOT_NAME, "RemoteShutdown_Udp_Client");
                }
                RaisePropertyChanged("Boot");
            }
        }

        #endregion

        #region 构造方法

        private SettingVM() { }

        #endregion
    }
}
