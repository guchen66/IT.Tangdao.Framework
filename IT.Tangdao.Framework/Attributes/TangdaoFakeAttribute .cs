using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Faker;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TangdaoFakeAttribute : Attribute
    {
        /// <summary>
        /// 用来描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 用来指定输出长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 默认值,仅用于string类型使用
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 用于指定bool值是否随机
        /// </summary>
        public bool Random { get; set; }

        #region 值类型使用

        /// <summary>
        /// 最小值，默认0
        /// </summary>
        public int Min { get; set; } = 0;

        /// <summary>
        /// 最大值，默认1000
        /// </summary>
        public int Max { get; set; } = 1000;

        /// <summary>
        /// 小数位数，默认4
        /// </summary>
        public int Point { get; set; } = 4;

        /// <summary>
        /// 是否为主键自增属性
        /// </summary>
        /// <remarks>
        /// 当设置为true时，该属性会自动生成连续递增的Id值
        /// 结合IsIdProperty方法自动识别Id属性
        /// </remarks>
        [Description("是否为主键自增属性")]
        public bool PrimarykeyAutoIncrement { get; set; } = false;

        #endregion 值类型使用

        /// <summary>
        /// 指定生成模板，请使用 <see cref="MockTemplate"/> 里的常量：
        /// <para> 示例：Template = nameof(MockTemplate.ChineseName) </para>
        /// <para> 可选常量：ChineseName, Mobile, City, Date, Email ... </para>
        /// </summary>
        [Description("使用预定义的数据模板")]
        public string Template { get; set; }
    }
}