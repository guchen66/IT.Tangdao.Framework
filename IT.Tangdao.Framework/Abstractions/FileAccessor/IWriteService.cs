namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 定义写入文本的服务
    /// </summary>
    public interface IWriteService
    {
        /// <summary>
        /// 默认写入内容接口
        /// </summary>
        IContentWritable Default { get; }
    }
}