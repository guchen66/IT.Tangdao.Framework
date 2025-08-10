using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.DaoAdmin
{
    /// <summary>
    /// 高级的读取接口，可以读取xml json config文件类型
    /// </summary>
    public interface IRead
    {
        string XMLData { get; set; }

        string JsonData { get; set; }

        string ConfigData { get; set; }

        IReadResult SelectNode(string node);

        IReadResult SelectNodes(string path);

        IReadResult<List<T>> SelectNodes<T>() where T : new();

        IReadResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector);

        IReadResult SelectKeys();

        /// <summary>
        /// 跟据Key读取Value
        /// 用于数组读取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IReadResult SelectValue(string key);

        /// <summary>
        /// 读取Json对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="Result"></param>
        /// <returns></returns>
       // IReadResult SelectJsonObject<TResult>(TResult @Result);

        IReadResult SelectConfig(string section);

        IReadResult SelectConfig<T>(string section) where T : class, new();

        IReadResult SelectConfigByJsonConvert<T>(string section) where T : class, new();

        IReadResult SelectCustomConfig(string configName, string section);

        IRead this[string readObject] { get; }
        IRead this[int readIndex] { get; }

        void Load(string data);
    }
}