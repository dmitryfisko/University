using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Forms;

namespace Lab_7.Resources
{
    public class CultureResources
    {
        private static bool _bFoundInstalledCultures;

        private static List<CultureInfo> SupportedCultures { get; } = new List<CultureInfo>();

        public CultureResources()
        {
            if (_bFoundInstalledCultures) return;
            foreach (var dir in Directory.GetDirectories(Application.StartupPath))
            {
                try
                {
                    var dirinfo = new DirectoryInfo(dir);
                    var tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);

                    if (dirinfo.GetFiles(Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".resources.dll").Length > 0)
                    {
                        SupportedCultures.Add(tCulture);
                    }
                }
                catch(ArgumentException)
                {
                }
            }
            _bFoundInstalledCultures = true;
        }

        public static Strings GetResourceInstance()
        {
            return new Strings();
        }

        private static ObjectDataProvider _mProvider;
        public static ObjectDataProvider ResourceProvider => _mProvider ?? (_mProvider = (ObjectDataProvider) App.Current.FindResource("Strings"));

        public static void ChangeCulture(CultureInfo culture)
        {
            if (!SupportedCultures.Contains(culture))
            {
               culture = Properties.Settings.Default.DefaultCulture;
            }
            Strings.Culture = culture;
            ResourceProvider.Refresh();
        }
    }
}
