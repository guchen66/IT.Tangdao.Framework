using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public class IniHelper
    {
        private NameValueCollection _values;
        private string[] keys;
        private string DefaultPath { get; set; }

        public IniHelper()
        {
           // DefaultPath = DirEx.CurrentDir() + "Config\\Setting.ini";                 //默认读取PLC的路径
        }

        // 链式调用：SelectSection 方法现在返回 PlcProvider 实例
     /*   public IniHelper SelectSection(string section)
        {
            IniFile ini = new IniFile(DefaultPath);
            _values = ini.GetSectionValues(section);
            return this; // 返回当前实例
        }

        public IniHelper SelectValue(string section)
        {
            IniFile ini = new IniFile(DefaultPath);
            keys = ini.GetKeys(section);
            return this; // 返回当前实例
        }

        // 链式调用：GetKey 方法现在返回 string 类型
        public string GetKey(string key)
        {
            if (_values != null && _values.AllKeys.Contains(key))
            {
                return _values[key]; // 返回找到的键对应的值
            }
            return null; // 如果键不存在，返回null
        }

        public void WriteValue(string key, string value)
        {
            IniFile ini = new IniFile(DefaultPath);

            ini.Write("PlcInfo2", key, value);
            ini.UpdateFile();
        }*/

        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        /// 写入INI的方法
        public void INIWrite(string section, string key, string value, string path)
        {
            // section=配置节点名称，key=键名，value=返回键值，path=路径
            WritePrivateProfileString(section, key, value, path);
        }

        //读取INI的方法
        public string INIRead(string section, string key, string path)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);

            // section=配置节点名称，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();

        }

    }
}
