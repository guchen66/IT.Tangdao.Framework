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
using IT.Tangdao.Framework.Abstractions.Loggers;
using System.Runtime.Remoting.Contexts;
using Newtonsoft.Json;
using IT.Tangdao.Framework.Paths;
using System.Windows.Markup;
using IT.Tangdao.Framework.Ambient;

namespace IT.Tangdao.Framework.Extensions
{
    public static class CacheContentExtensions
    {
        /// <summary>
        /// 从缓存反序列化（同步） - string 路径
        /// </summary>
        public static T DeserializeCache<T>(this ICacheContentQueryable cache, string path, DaoFileType type = DaoFileType.None) where T : class, new()
            => ResolveInternal<T>(cache, path, type);

        /// <summary>
        /// 从缓存反序列化（同步） - AbsolutePath 路径
        /// </summary>
        public static T DeserializeCache<T>(this ICacheContentQueryable cache, AbsolutePath path, DaoFileType type = DaoFileType.None) where T : class, new()
            => ResolveInternal<T>(cache, path.Value, type);

        #region 私有实现

        private static T ResolveInternal<T>(ICacheContentQueryable cache, string fullPath, DaoFileType type) where T : class, new()
        {
            try
            {
                var rootKey = FileContentCacheKey.Create(fullPath, type);
                var parameter = TangdaoContext.GetTangdaoParameter(rootKey);
                string content = parameter.Get<string>(rootKey);

                var detected = FileHelper.DetectFromContent(content);

                switch (detected)
                {
                    case DaoFileType.Xml:
                        return XmlFolderHelper.Deserialize<T>(content);

                    case DaoFileType.Json:
                        return JsonConvert.DeserializeObject<T>(content);

                    case DaoFileType.Config:
                        return ConfigFolderHelper.DeserializeObject<T>(content);

                    default:
                        throw new NotSupportedException($"不支持的文件类型: {detected}");
                }
            }
            catch
            {
                return default(T);
            }
        }

        #endregion 私有实现
    }
}