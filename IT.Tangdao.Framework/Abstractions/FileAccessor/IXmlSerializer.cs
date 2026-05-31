using System;
using System.Xml.Linq;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IXmlSerializer
    {
        void ToXml<T>(T obj);
    }
}