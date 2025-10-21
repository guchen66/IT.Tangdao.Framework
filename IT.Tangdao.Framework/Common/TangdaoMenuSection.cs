using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common
{
    public class TangdaoMenuSection : ConfigurationSection
    {
        [ConfigurationProperty("menus", IsRequired = true)]
        public MenuSectionElementCollection Menus
        {
            get { return (MenuSectionElementCollection)this["menus"]; }
        }
    }

    public class MenuSectionElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MenuElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            // 使用title属性作为元素的键
            return ((MenuElement)element).Title;
        }
    }

    public class MenuElement : ConfigurationElement
    {
        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get { return (string)this["title"]; }
            set { this["title"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}