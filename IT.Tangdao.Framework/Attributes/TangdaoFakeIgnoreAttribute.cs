using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    /// <summary>
    /// 批量假数据排除标记（类级）。
    /// <para>
    /// 用于让某个具体类退出"按程序集批量生成"：<c>ITangdaoFakeEntity</c> 标记只能"加"不能"减"，
    /// 当一个类沿继承链拿到了实体标记、但本身不希望被造值时，在此类上标注本特性即可。
    /// </para>
    /// <para>
    /// 【Inherited = true】继承传递：标注在抽象基类上时，其具体子类一并退出批量生成，
    /// 使"排除"与"标记"在继承链上的传递方向保持一致。
    /// </para>
    /// <para>
    /// 本特性只影响<b>扫描结果</b>（即哪些类参与批量生成），被排除的类仍可通过
    /// <c>TangdaoDataFaker&lt;T&gt;.Build</c> 单独造值。
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TangdaoFakeIgnoreAttribute : Attribute
    {
    }
}
