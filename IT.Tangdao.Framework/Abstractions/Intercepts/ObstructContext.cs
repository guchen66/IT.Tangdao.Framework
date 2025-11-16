using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Intercepts
{
    public class ObstructContext
    {
        public object Target { get; set; }           // 目标对象(Student实例)
        public string MethodName { get; set; }       // 方法名
        public object[] Arguments { get; set; }      // 方法参数
        public object ReturnValue { get; set; }      // 返回值
        public bool IsProceed { get; set; } = true;  // 是否继续执行原方法

        // 获取参数值的便捷方法
        public T GetArgument<T>(int index)
        {
            return Arguments != null && index < Arguments.Length ? (T)Arguments[index] : default(T);
        }

        public void SetArgument(int index, object value)
        {
            if (Arguments != null && index < Arguments.Length)
            {
                Arguments[index] = value;
            }
        }
    }
}