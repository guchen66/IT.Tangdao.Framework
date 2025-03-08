using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.DaoAdmin
{
    /// <summary>
    /// 高级的读取接口，可以读取除了txt文件，xml文件之外的其他类型
    /// </summary>
    public interface IRead
    {
        string XMLData { get; set; }

        string JsonData { get; set; }

        IReadResult SelectNode(string text);

        IReadResult SelectNodes(string path);

        IReadResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector);

        IReadResult<List<T>> Descendants<T>(string rootElement, Func<XElement, T> selector) where T : class;

        IReadResult SelectKeys();

        IReadResult SelectValue(string key);

        IReadResult SelectConfig(string section);

        IReadResult SelectCustomConfig(string configName, string section);

        IRead this[string readObject] { get; }
    }
}