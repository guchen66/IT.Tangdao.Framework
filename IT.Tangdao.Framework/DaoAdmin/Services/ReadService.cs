using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoSelectors;
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

namespace IT.Tangdao.Framework.DaoAdmin.Services
{
    public class ReadService : IReadService
    {
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

        public Task<string> QueryFilterAsync(string path, Expression<Func<string, bool>> func)
        {
            throw new NotImplementedException();
        }

        private async Task<string> ReadTxtAsync(string path)
        {
            using (var stream = new StreamReader(path.UseFileOpenRead()))
            {
                return await stream.ReadToEndAsync();
            }
        }

        private async Task<string> ReadXmlAsync(string path)
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

        public IRead Current => GetFile();

        private IRead GetFile()
        {
            return FileSelector.Queryable();
        }

        public IHardwaredevice Device => GetDevice();

        private IHardwaredevice GetDevice()
        {
            return DeviceSelector.SelectCurrentMatchDevice("");
        }
    }
}