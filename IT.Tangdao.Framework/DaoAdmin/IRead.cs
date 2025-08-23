using IT.Tangdao.Framework.DaoAdmin.Results;
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

        ReadResult SelectNode(string node);

        ReadResult SelectNodes(string path);

        ReadResult<List<T>> SelectNodes<T>() where T : new();

        ReadResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector);

        ReadResult SelectKeys();

        /// <summary>
        /// 跟据Key读取Value
        /// 用于数组读取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ReadResult SelectValue(string key);

        /// <summary>
        /// 读取Json对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="Result"></param>
        /// <returns></returns>
       // ReadResult SelectJsonObject<TResult>(TResult @Result);

        ReadResult SelectConfig(string section);

        ReadResult SelectConfig<T>(string section) where T : class, new();

        ReadResult SelectConfigByJsonConvert<T>(string section) where T : class, new();

        ReadResult SelectCustomConfig(string configName, string section);

        IRead this[string readObject] { get; }
        IRead this[int readIndex] { get; }

        void Load(string data);
    }
}