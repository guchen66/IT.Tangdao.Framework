using IT.Tangdao.Framework.Abstractions.Results;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentWritable
    {
        /// <summary>
        /// 写入的内容
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 写入的地址
        /// </summary>
        string WritePath { get; }

        /// <summary>
        /// 文件类型
        /// </summary>
        DaoFileType DetectedType { get; }
    }
}