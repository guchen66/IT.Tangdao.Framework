using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 工厂唯一职责：根据注册项，实例化一个对象。
    /// 实现者可反射、可表达式树、可 IL，外部无感知。
    /// </summary>
    public interface IServiceFactory
    {
        object Create(IServiceEntry entry);
    }
}