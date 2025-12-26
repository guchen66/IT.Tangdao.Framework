namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 内容读取器
    /// </summary>
    public interface IContentAccess
    {
        IContentBuilder Default { get; }
        ICacheQueryBuilder Cache { get; }
    }
}