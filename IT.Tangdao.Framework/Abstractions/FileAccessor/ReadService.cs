using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.Abstractions
{
    /// <inheritdoc/>
    public class ReadService : IReadService
    {
        /// <inheritdoc/>
        public string Read(string path, DaoFileType daoFileType = DaoFileType.None)
        {
            string content = string.Empty;

            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            content = path.UseStreamReadToEnd();
            return content;
        }

        /// <inheritdoc/>
        public async Task<string> ReadAsync(string path, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            switch (daoFileType)
            {
                case DaoFileType.Txt:
                    return await ReadTxtAsync(path);

                case DaoFileType.Xml:
                    return await ReadXmlAsync(path);
                // 可以继续添加其他文件类型的处理逻辑
                default:
                    throw new NotSupportedException($"不支持的文件类型: {daoFileType}");
            }
        }

        /// <inheritdoc/>
        public async Task<TEntity> ReadXmlToEntityAsync<TEntity>(string path, DaoFileType daoFileType) where TEntity : class, new()
        {
            string xml = string.Empty;
            TEntity Entity = new TEntity();
            if (daoFileType == DaoFileType.Xml)
            {
                xml = await ReadXmlAsync(path);
                Entity = XmlFolderHelper.Deserialize<TEntity>(xml);
            }
            return Entity;
        }

        public ReadResult<string> BatchReadFileAsync(string path, DaoFileType daoFileType = DaoFileType.Txt)
        {
            if (string.IsNullOrWhiteSpace(path))
                return ReadResult<string>.Failure("路径不能为空。");

            if (FileSelector.GetPathKind(path) != PathKind.Directory)
                return ReadResult<string>.Failure("指定路径必须是有效目录。");

            // 1. 先拿过滤后的文件列表（私有方法，见下）
            var files = QueryFilter(path, daoFileType);
            if (!files.Any())
                return ReadResult<string>.Failure($"目录下未找到 {daoFileType} 类型文件。");

            // 2. 逐个读并合并
            var sb = new StringBuilder();
            foreach (var file in files)
            {
                try
                {
                    sb.AppendLine(File.ReadAllText(file));
                }
                catch (Exception ex)
                {
                    return ReadResult<string>.Failure($"读取文件失败：{ex.Message}");
                }
            }
            return ReadResult<string>.Success(sb.ToString());
        }

        /// <summary>
        /// 私有过滤：只返回指定后缀的文件路径。
        /// </summary>
        private static IEnumerable<string> QueryFilter(string directoryPath, DaoFileType type)
        {
            switch (type)
            {
                case DaoFileType.None:
                    break;

                case DaoFileType.Txt:
                    break;

                case DaoFileType.Xml:
                    break;

                case DaoFileType.Xlsx:
                    break;

                case DaoFileType.Xaml:
                    break;

                case DaoFileType.Json:
                    break;

                case DaoFileType.Config:
                    break;

                default:
                    break;
            }

            return Directory.EnumerateFiles(directoryPath, $"*{type}", SearchOption.AllDirectories);
        }

        private static async Task<string> ReadTxtAsync(string path)
        {
            using (var stream = new StreamReader(path.UseFileOpenRead()))
            {
                return await stream.ReadToEndAsync();
            }
        }

        private static async Task<string> ReadXmlAsync(string path)
        {
            using (var stream = new StreamReader(path.UseFileOpenRead()))
            {
                var xmlContent = await stream.ReadToEndAsync();
                // 这里可以根据需要对xmlContent进行解析
                // 例如，使用XDocument加载XML内容
                var doc = XDocument.Parse(xmlContent);
                // 对doc进行处理，例如提取数据
                // ...
                // 返回处理后的字符串或XML文档的字符串表示
                return doc.ToString();
            }
        }

        /// <inheritdoc/>
        public IRead Current => new Read();

        public IContentQueryable Default => new ContentQueryable();
    }
}