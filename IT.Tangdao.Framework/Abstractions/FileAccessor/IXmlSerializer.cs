using System;
using System.Xml.Linq;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IXmlSerializer
    {
        void ToXml<T>(T obj);
    }
}