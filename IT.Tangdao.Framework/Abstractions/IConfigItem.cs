using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface IConfigItem
    {
        string Key { get; set; }

        string Value { get; set; }
    }

    public interface IConfigCollection
    {
        List<IConfigItem> ConfigItems { get; set; }
    }

    public class ConfigElement
    {
    }
}