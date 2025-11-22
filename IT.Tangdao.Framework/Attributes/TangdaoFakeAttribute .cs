using System;
using System.Collections.Generic;
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

        public int Min { get; set; }

        public int Max { get; set; }

        public int Point { get; set; }

        #endregion 值类型使用

        /// <summary>
        /// 指定生成模板，请使用 <see cref="MockTemplate"/> 里的常量：
        /// <para> 示例：Template = nameof(MockTemplate.ChineseName) </para>
        /// <para> 可选常量：ChineseName, Mobile, City, Date, Email ... </para>
        /// </summary>
        public string Template { get; set; }
    }
}