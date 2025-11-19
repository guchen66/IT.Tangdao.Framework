using IT.Tangdao.Framework.Abstractions.Results;
using System;
using System.IO;
using System.Linq;
using IT.Tangdao.Framework.Helpers;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    internal sealed class ContentWritable : IContentWritable
    {
        //public object WriteObject
        //{
        //    get => _writeObject;
        //    set => _writeObject = value;
        //}

        //private object _writeObject;

        //// 实现接口中的索引器
        //public IContentWritable this[object writeObject]
        //{
        //    get
        //    {
        //        _writeObject = writeObject;
        //        return this;
        //    }
        //}

        /// <summary>
        /// 写入内容
        /// </summary>
        public void Write(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            path.UseFileWriteToTxt(content);
        }

        /// <summary>
        /// 异步写入内容
        /// </summary>
        public async Task<ResponseResult> WriteAsync(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            await new TimeSpan(1000);
            path.UseFileWriteToTxt(content);
            return ResponseResult<string>.Success(content);
        }

        /// <summary>
        /// 序列化对象并写入
        /// </summary>
        public void WriteObject<T>(string path, T obj)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("路径不能为空", nameof(path));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "要序列化的对象不能为null");

            try
            {
                // 根据文件扩展名决定序列化格式
                var extension = Path.GetExtension(path)?.ToLowerInvariant();
                // string content;

                switch (extension)
                {
                    case ".xml":
                        TangdaoXmlSerializer.SerializeXMLToFile<T>(obj, path);
                        break;

                    case ".json":
                        TangdaoJsonFileHelper.SaveJsonData(obj, path);
                        break;

                    default:
                        // content = JsonConvert.SerializeObject(obj, _jsonSettings);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"序列化并写入对象失败: {path}", ex);
            }
        }

        //protected internal void Serialize()
        //{
        //    XmlFolderHelper.SerializeXMLToFile();
        //}
    }
}