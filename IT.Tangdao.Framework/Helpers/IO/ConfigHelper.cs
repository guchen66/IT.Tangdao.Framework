using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public class ConfigHelper
    {
        public static string ReadConfig(string key)
        {
            Configuration congfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return congfig.AppSettings.Settings[key].Value.ToString();
        }

        public static void SaveConfig(string key, string value)
        {
            Configuration congfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            congfig.AppSettings.Settings[key].Value = value;
            congfig.Save();
        }
    }
}