using System.Windows;
using System;
using System.Collections.Generic;

namespace RemoteShutdown.Core
{
    public static class ResourcesHelper
    {
        public static string GetValue(string key, string defualtValue = null)
        {
            if (Application.Current.Resources.Contains(key))
            {
                return (string)Application.Current.Resources[key];
            }
            return defualtValue;
        }

        public static void SetLanguage(string language)
        {
            language = ConvertLanguage(language);
            var cultureInfo = new System.Globalization.CultureInfo(language);
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

            Application.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary resource = (ResourceDictionary)Application.LoadComponent(
                new Uri(string.Format("Resources/lang/{0}.xaml", language),
                    UriKind.Relative));
            Application.Current.Resources.MergedDictionaries.Add(resource);


        }

        public static string ConvertLanguage(string language)
        {
            if (language.IndexOf("en", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                language = "en-US";
            }
            else
            {
                language = "zh-CN";
            }
            return language;
        }

        public static IEnumerable<Language> GetLanguages()
        {
            yield return new Language { DisplayName = "简体中文", Name = "zh-CN" };
            yield return new Language { DisplayName = "English", Name = "en-US" };
        }

        public class Language
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }
    }
}
