using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.EventArg;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 通用通知发布者实现，支持发布任意类型的通知给订阅者
    /// </summary>
    public class TangdaoPublisher : TangdaoPublisher<object>, ITangdaoPublisher
    {
    }

    /// <summary>
    /// 空的Disposable实现
    /// </summary>
    internal static class Disposable
    {
        public static readonly IDisposable Empty = new EmptyDisposable();

        private sealed class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
                // 空实现
            }
        }
    }
}