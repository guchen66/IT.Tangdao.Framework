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
            // 路径先规范化为绝对路径，保证失败时异常与结果里都能定位到具体文件
            var fullPath = path;
            try
            {
                fullPath = Path.GetFullPath(path);

                // 读取文件内容
                var content = File.ReadAllText(fullPath);
                var detectedType = t == DaoFileType.None ? FileQueryable.GetExtension(fullPath) : t;

                // 缓存内容
                var rootKey = string.Format("Content:{0}:{1}", fullPath, detectedType);
                TangdaoParameter tangdaoParameter = new TangdaoParameter();
                tangdaoParameter.Add(rootKey, content);    //缓存内容
                TangdaoContext.SetTangdaoParameter(rootKey, tangdaoParameter);

                return new ContentQueryable
                {
                    Content = content,
                    ReadPath = fullPath,
                    DetectedType = detectedType,
                    Status = ResponseResult.Success("读取成功", fullPath)
                };
            }
            catch (Exception ex)
            {
                // 读取失败不再抛出：原因随结果对象一起向下游传递
                return new ContentQueryable
                {
                    Content = null,
                    ReadPath = fullPath,
                    DetectedType = t,
                    Status = ResponseResult.Failure($"读取失败：{fullPath}", ex, fullPath)
                };
            }
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