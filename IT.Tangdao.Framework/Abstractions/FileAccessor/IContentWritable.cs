using IT.Tangdao.Framework.Abstractions.Results;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentWritable
    {
        /// <summary>
        /// 写入内容
        /// </summary>
        void Write(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 异步写入内容
        /// </summary>
        Task<WriteResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None);

        /// <summary>
        /// 序列化对象并写入
        /// </summary>
        void WriteObject<T>(string path, T obj);

        // IContentWritable this[object writeObject] { get; }
    }
}