using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    public interface INoticeFactory
    {
        INoticeObserver CreateObserver(NoticeContext naviceContext);
    }

    public sealed class DefaultNoticeFactory : INoticeFactory
    {
        // key → 类型的内部映射表
        private static readonly Dictionary<string, Type> _typeMap =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "Badge", typeof(BadgeObserver) },
               // { "Alert", typeof(AlertObserver) }
                // 用户如果写了自己的 XXXObserver，在这里注册一行即可
            };

        public INoticeObserver CreateObserver(NoticeContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var key = context.Key;
            if (!_typeMap.TryGetValue(key, out var type))
                throw new NotSupportedException(
                    $"No observer registered for key '{key}'. " +
                    $"Call DefaultNoticeFactory.Register<{key}>() at module initializer.");

            // 零参数构造
            return (INoticeObserver)Activator.CreateInstance(type);
        }

        // 允许外部代码（通常是他们自己的模块初始化器）注入新类型
        public static void Register<T>(string key) where T : INoticeObserver
        {
            _typeMap[key] = typeof(T);
        }
    }
}