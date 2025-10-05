using IT.Tangdao.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ConfigItemExtension
    {
        /// <summary>
        /// 读取WPF自带的App.config
        /// 这两个引用没有传递值，是读取config的值，所以不需要使用ref，
        /// 使用了struct后，如果传递数据的扩展方法，需要加上ref
        /// </summary>
        /// <param name="source"></param>
        public static Dictionary<string, string> ReadAppConfig<TSource>(this TSource source, string section) where TSource : IConfigItem
        {
            IDictionary idict = (IDictionary)ConfigurationManager.GetSection(section);
            Dictionary<string, string> dict = idict.Cast<DictionaryEntry>().ToDictionary(de => de.Key.ToString(), de => de.Value.ToString());
            return dict;
        }

        /// <summary>
        /// 读取自定义的config文件
        /// </summary>
        /// <param name="source"></param>
        public static Dictionary<string, string> ReadUnityConfig<TSource>(this TSource source, string section) where TSource : IConfigItem
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"unity.config")
            };

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

           /* var customSection = (IConfigItem)config.GetSection(section);
            if (customSection == null)
            {
                dicts.Add("null", null);
                return dicts;
            }
            foreach (MenuElement menu in customSection.Menus)
            {
                dicts.TryAdd(menu.Title, menu.Value);
            }*/
            return dicts;

        }
    }
}
