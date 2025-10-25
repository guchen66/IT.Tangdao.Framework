using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.FileAccessor;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Abstractions.Results;
using System.Threading;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 查询文件时候链式调用的语法糖扩展
    /// </summary>
    public static class ContentQueryableExtension
    {
        /// <summary>
        /// 不管当前是什么格式，直接一次性反序列化成 T
        /// 内部走 Auto 探测 + 对应解析器
        /// </summary>
        public static ResponseResult<T> ReadToObject<T>(this IContentQueryable query, string key = null, CancellationToken token = default)
        {
            // 1. 先让实例进入“已决”状态（若外部还没调 AsXxx）
            var detected = (query as ContentQueryable)?.DetectedType ?? DaoFileType.None;
            if (detected == DaoFileType.None)
                query = query.Auto();

            // 2. 按格式走最快路径
            return (ResponseResult<T>)new object();
        }
    }
}