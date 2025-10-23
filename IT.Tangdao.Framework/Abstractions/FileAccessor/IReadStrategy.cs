using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IReadStrategy
    {
        DaoFileType SupportType { get; }

        bool CanHandle(string contentOrPath);

        ReadResult<T> Read<T>(string content, string key = null) where T : class, new();
    }

    //internal sealed class XmlReadStrategy : IReadStrategy
    //{
    //    public DaoFileType SupportType => DaoFileType.Xml;
    //    public bool CanHandle(string content) => content.TrimStart().StartsWith("<");

    //    public ReadResult<T> Read<T>(string content, string key = null) where T : class, new()
    //    {
    //        // 把原来 Read.SelectNodes<T>() 的逻辑搬过来
    //    }
    //}

    //internal sealed class JsonReadStrategy : IReadStrategy
    //{
    //    public DaoFileType SupportType => DaoFileType.Json;
    //    public bool CanHandle(string content) => content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("[");

    //    public ReadResult<T> Read<T>(string content, string key = null) where T : class, new()
    //    {
    //        // 原 Read.SelectValue / SelectNodes 逻辑
    //    }
    //}
}