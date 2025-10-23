namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 定义读取文本的服务
    /// </summary>
    public interface IReadService
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