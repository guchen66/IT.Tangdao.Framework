using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Threading;
using System.Configuration;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Abstractions.FileAccessor;
using System;
using System.Globalization;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Abstractions.Loggers;
using System.Runtime.Remoting.Contexts;
using IT.Tangdao.Framework.Infrastructure.Ambient;
using Newtonsoft.Json;

namespace IT.Tangdao.Framework.Extensions
{
    public static class CacheContentExtensions
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(CacheContentExtensions));

        /// <summary>
        /// 从缓存反序列化（同步）
        /// </summary>
        public static T Deserialize<T>(this ICacheContentQueryable cache, string path, DaoFileType type = DaoFileType.None) where T : class, new()
        {
            try
            {
                var rootKey = CacheKey.GetCacheKey(path, type);

                var parameter = TangdaoContext.GetTangdaoParameter(rootKey);

                string Data = parameter.Get<string>(rootKey);
                var detected = FileSelector.DetectFromContent(Data);
                T result = new T();

                switch (detected)
                {
                    case DaoFileType.Xml:
                        result = XmlFolderHelper.Deserialize<T>(Data);
                        break;

                    case DaoFileType.Json:
                        result = JsonConvert.DeserializeObject<T>(Data); ;
                        break;

                    case DaoFileType.Config:
                        result = ReadResult.Success(Data).ToObject<T>();
                        break;

                    default:
                        throw new NotSupportedException($"不支持的文件类型: {detected}");
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;// ReadResult<T>.FromException(ex);
            }
        }
    }
}