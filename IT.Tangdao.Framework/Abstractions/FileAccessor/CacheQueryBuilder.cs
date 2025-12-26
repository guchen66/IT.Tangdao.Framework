using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Helpers;
using IT.Tangdao.Framework.Extensions;
using System;
using System.IO;
using IT.Tangdao.Framework.Paths;
using IT.Tangdao.Framework.Ambient;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <summary>
    /// 缓存查询构建器实现
    /// </summary>
    public class CacheQueryBuilder : ICacheQueryBuilder
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(CacheQueryBuilder));

        public IContentQueryable Read(string path, DaoFileType t = DaoFileType.None)
        {
            var rootKey = FileContentCacheKey.Create(path, t);

            // ① TangdaoContext 拿实例级缓存
            var hit = TangdaoContext.GetInstance<ContentQueryable>(rootKey);
            if (hit != null)
            {
                return hit;
            }

            // ② 磁盘读 + 探测
            var content = File.ReadAllText(path);
            var detectedType = t == DaoFileType.None ? FileHelper.DetectFromContent(content) : t;

            // ③ 新实例（无参构造）
            var fresh = new ContentQueryable
            {
                Content = content
            };

            // ④ 放进 TangdaoContext 缓存桶
            TangdaoContext.SetInstance(rootKey, fresh);

            // ⑤ 缓存内容
            var parameter = new TangdaoParameter();
            parameter.Add(rootKey, content);    //缓存内容
            TangdaoContext.SetTangdaoParameter(rootKey, parameter);

            return fresh;
        }

        public IContentQueryable Read(AbsolutePath path, DaoFileType t = DaoFileType.None)
        {
            return Read(path.Value, t);
        }

        public IContentQueryable Empty()
        {
            // 直接返回一个空的 ContentQueryable 实例
            return new ContentQueryable();
        }

        public void Clear(string path)
        {
            var key = FileContentCacheKey.Create(path, DaoFileType.None);
            TangdaoContext.SetInstance(key, null);   // 清除缓存实例
            TangdaoContext.SetTangdaoParameter(key, null); // 清除缓存内容
        }

        public void ClearRegion(string region)
        {
            throw new NotImplementedException();
        }

        public IContentWritable Write(string path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            throw new NotImplementedException();
        }

        public IContentWritable Write(AbsolutePath path, string content, DaoFileType daoFileType = DaoFileType.None)
        {
            throw new NotImplementedException();
        }
    }
}