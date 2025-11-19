namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 内容写入器
    /// </summary>
    public interface IContentWriter
    {
        /// <summary>
        /// 默认写入内容接口
        /// </summary>
        IContentWritable Default { get; }
    }
}