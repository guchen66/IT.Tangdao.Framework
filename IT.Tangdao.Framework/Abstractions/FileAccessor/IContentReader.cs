namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 内容读取器
    /// </summary>
    public interface IContentReader
    {
        /// <summary>
        /// 默认读取内容接口
        /// </summary>
        IContentQueryable Default { get; }

        /// <summary>
        /// 缓存读取数据接口
        /// </summary>
        ICacheContentQueryable Cache { get; }
    }
}