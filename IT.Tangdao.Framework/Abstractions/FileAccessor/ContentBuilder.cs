using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Paths;
using System.Xml;
using IT.Tangdao.Framework.Configurations;
using System.Runtime.Remoting.Contexts;
using IT.Tangdao.Framework.Infrastructure;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public class ContentBuilder : IContentBuilder
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(ContentBuilder));

        public IContentQueryable Empty()
        {
            return ContentQueryable.CreateInstance();
        }

        public IContentQueryable Read(string path, DaoFileType t = DaoFileType.None)
        {
            // 读取文件内容
            var content = File.ReadAllText(path);
            var detectedType = t == DaoFileType.None ? FileQueryable.GetExtension(path) : t;

            // 缓存内容
            var rootKey = string.Format("Content:{0}:{1}", path, detectedType);
            TangdaoParameter tangdaoParameter = new TangdaoParameter();
            tangdaoParameter.Add(rootKey, content);    //缓存内容
            TangdaoContext.SetTangdaoParameter(rootKey, tangdaoParameter);
            return new ContentQueryable { Content = content, ReadPath = path, DetectedType = detectedType };
        }

        public IContentQueryable Read(AbsolutePath path, DaoFileType t = DaoFileType.None)
        {
            return Read(path.Value, t);
        }

        /// <summary>
        /// 写入内容
        /// </summary>
        public IContentWritable Write(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            path.UseFileWriteToTxt(content);
            return new ContentWritable() { Content = content, DetectedType = daoFileType, WritePath = path };
        }

        public IContentWritable Write(AbsolutePath path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            path.Value.UseFileWriteToTxt(content);
            return new ContentWritable() { Content = content, DetectedType = daoFileType, WritePath = path.Value };
        }
    }
}