using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 批量假数据实体标记接口。
    /// <para>
    /// 【职责】只做一件事：声明"本类参与按程序集批量生成假数据"。
    /// 本接口不含任何成员，库也不会为它生成实现，扫描阶段仅通过
    /// <c>IsAssignableFrom</c> 识别它的实现者。
    /// </para>
    /// <para>
    /// 【为什么用接口而不是特性】接口的标记天然沿继承链传递：把标记打在抽象基类上，
    /// 所有具体子类无需重复标注即可被统一召回。库内 <c>WindowFactory.Initialize</c>
    /// 对 <c>IGuardAware</c> 的判定就是同一范式。
    /// </para>
    /// <para>
    /// 【如何"反标记"】接口只能加不能减，个别具体类若需退出批量生成，
    /// 在其上标注 <c>TangdaoFakeIgnoreAttribute</c> 即可。
    /// </para>
    /// <para>
    /// 【扫描口径】抽象类自身不会被纳入（扫描会排除抽象类），只有具体子类参与生成；
    /// 开放泛型、无公共无参构造的类同样被排除。
    /// </para>
    /// </summary>
    public interface ITangdaoFakeEntity
    {
    }
}
