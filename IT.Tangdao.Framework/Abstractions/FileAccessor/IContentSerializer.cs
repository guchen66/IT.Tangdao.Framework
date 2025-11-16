using System;
using System.Xml.Linq;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentSerializer
    {
        DaoFileType SupportType { get; }

        T Deserialize<T>(string content) where T : class, new();

        string Serialize<T>(T obj);
    }

    internal sealed class XmlSerializerStrategy : IContentSerializer
    {
        public DaoFileType SupportType
        {
            get { return DaoFileType.Xml; }
        }

        public T Deserialize<T>(string content) where T : class, new()
        {
            var doc = XDocument.Parse(content);
            T instance = new T();
            FileHelper.MapXElementToObject(doc.Root, instance);
            return instance;
        }

        public string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }
}