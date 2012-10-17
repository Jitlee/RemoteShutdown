using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace RemoteShutdown.Utilities
{
    public class RWReg
    {
        public static object GetValue(string subName, string keyName, object defualtValue = null)
        {
            using (var rootKey = Registry.CurrentUser)
            {
                using (var subKey = rootKey.OpenSubKey(subName))
                {
                    if (null == subKey)
                    {
                        return defualtValue;
                    }
                    return subKey.GetValue(keyName, defualtValue);
                }
            }
        }

        public static void SetValue(string subName, string keyName, object value)
        {
            using (var rootKey = Registry.CurrentUser)
            {
                using (var subKey = rootKey.OpenSubKey(subName, true))
                {
                    if (null == subKey)
                    {
                        using (var newSubKey = rootKey.CreateSubKey(subName))
                        {
                            newSubKey.SetValue(keyName, value);
                        }
                    }
                    else
                    {
                        subKey.SetValue(keyName, value);
                    }
                }
            }
        }

        public static void SetValue(string subName, string keyName, object value, RegistryValueKind valueKind)
        {
            using (var rootKey = Registry.CurrentUser)
            {
                using (var subKey = rootKey.OpenSubKey(subName, true))
                {
                    if (null == subKey)
                    {
                        using (var newSubKey = rootKey.CreateSubKey(subName))
                        {
                            newSubKey.SetValue(keyName, value, valueKind);
                        }
                    }
                    else
                    {
                        subKey.SetValue(keyName, value, valueKind);
                    }
                }
            }
        }

        public static void RemoveKey(string subName, string keyName)
        {
            using (var rootKey = Registry.CurrentUser)
            {
                using (var subKey = rootKey.OpenSubKey(subName, true))
                {
                    if (null != subKey)
                    {
                        try
                        {
                            var key = subKey.OpenSubKey(keyName);
                            if (null != key)
                            {
                                subKey.DeleteValue(keyName);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
    }
}
