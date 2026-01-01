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
    internal sealed class ContentWritable : IContentWritable, IXmlSerializer, IConfigSerializer, IJsonSerializer, IIniSerializer
    {
        public string Content { get; set; }

        public string WritePath { get; set; }

        public DaoFileType DetectedType { get; set; }

        #region --格式切换--

        public IContentWritable Auto() => this;   // 逻辑已在内置字段，直接返回自己

        public IXmlSerializer AsXml()
        {
            if (DetectedType == DaoFileType.Xml)
            {
                return this;
            }
            throw new FormatException("转换失败，写入的内容不是有效的XML格式");
        }

        public IJsonSerializer AsJson()
        {
            if (DetectedType == DaoFileType.Json)
            {
                return this;
            }
            throw new FormatException("转换失败，写入的内容不是有效的Json格式");
        }

        public IConfigSerializer AsConfig()
        {
            // 如果是 Config 类型，或者是未写入状态（用于写入默认 App.config），都允许转换
            if (DetectedType == DaoFileType.Config || DetectedType == DaoFileType.None)
            {
                return this;
            }
            throw new FormatException("转换失败，写入的内容不是有效的Config格式");
        }

        public IIniSerializer AsIni()
        {
            if (DetectedType == DaoFileType.Ini)
            {
                return this;
            }
            throw new FormatException("转换失败，写入的内容不是有效的Ini格式");
        }

        #endregion --格式切换--

        public void ToXml<T>(T obj)
        {
            TangdaoXmlSerializer.SerializeXMLToFile<T>(obj, WritePath);
        }

        public void ToJson<T>(T obj)
        {
            TangdaoJsonFileHelper.SaveJsonData(obj, WritePath);
        }
    }
}