using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Extensions;
using System;
using IT.Tangdao.Framework.Infrastructure.Ambient;
using System.IO;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    internal sealed class CacheContentQueryable : ICacheContentQueryable
    {
        /* 依旧无参构造器 - DI 友好 */
        private string _content = string.Empty;

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public CacheContentQueryable()
        { }

        private ContentQueryable _inner = null;
        /* ========== 缓存版 Read - 用 new 隐藏父接口签名 ========== */

        public new IContentQueryable Read(string path, DaoFileType type)
        {
            var rootKey = CacheKey.GetCacheKey(path, type);

            var parameter = TangdaoContext.GetTangdaoParameter(rootKey);

            string Data = parameter.Get<string>(rootKey);
            var detected = FileSelector.DetectFromContent(Data);
            // ① TangdaoContext 拿实例级缓存
            var hit = TangdaoContext.GetInstance<ContentQueryable>(rootKey);
            if (hit != null)
            {
                _inner = hit;
                return hit;
            }

            // ② 磁盘读 + 探测
            var content = File.ReadAllText(path);

            // ③ 新实例（无参构造）
            var fresh = new ContentQueryable
            {
                Content = content,
                // DetectedType = detected
            };

            // ④ 放进 TangdaoContext 缓存桶
            TangdaoContext.SetInstance(rootKey, fresh);
            return fresh;
        }

        /* ========== 以下全部是 IContentQueryable 的显式实现 ========== */

        public IContentXmlQueryable AsXml() => _inner.AsXml();

        public IContentJsonQueryable AsJson() => _inner.AsJson();

        public IContentConfigQueryable AsConfig() => _inner.AsConfig();

        public IContentQueryable Auto() => this;

        public IContentQueryable this[int idx] => new ContentQueryable()[idx];
        public IContentQueryable this[string obj] => new ContentQueryable()[obj];

        /* ========== 缓存特有 ========== */

        public void Clear(string path)
        {
            var key = string.Format("CacheRead:{0}", path);
            TangdaoContext.SetInstance(key, null);   // 用新增的带 key 接口
        }

        public void ClearRegion(string region) =>
            throw new NotImplementedException();
    }
}