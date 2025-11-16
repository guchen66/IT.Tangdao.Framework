using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Intercepts
{
    // 拦截器接口 - 定义拦截生命周期
    public interface IInterceptHandler
    {
        void BeforeInvoke(ObstructContext context);

        void AfterInvoke(ObstructContext context);

        void OnError(ObstructContext context, Exception exception);
    }

    public class StringInterceptHandler : IInterceptHandler
    {
        public void BeforeInvoke(ObstructContext context)
        {
            if (context.Arguments.Length > 0 && context.Arguments[0] is string name)
            {
                // 剔除非法字符而不是抛出异常
                string cleanedName = RemoveIllegalCharacters(name);
                context.SetArgument(0, cleanedName);
            }
        }

        private string RemoveIllegalCharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var illegalChars = new HashSet<char> { '<', '>', '&', '\'', '\"', '\\', '/' };
            return new string(input.Where(c => !illegalChars.Contains(c)).ToArray());
        }

        public void AfterInvoke(ObstructContext context)
        { }

        public void OnError(ObstructContext context, Exception exception)
        { }
    }
}