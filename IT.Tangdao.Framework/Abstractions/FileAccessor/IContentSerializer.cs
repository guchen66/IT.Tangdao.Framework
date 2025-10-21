using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;

namespace IT.Tangdao.Framework.Abstractions
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
            FileSelector.MapXElementToObject(doc.Root, instance);
            return instance;
        }

        public string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }

    public static class ReadServiceProvider
    {
        private static readonly AsyncLocal<IReadService> _current = new AsyncLocal<IReadService>();

        public static IReadService Default { get; } = new ReadService();

        public static IReadService Current
        {
            get => _current.Value ?? Default;
            set => _current.Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}