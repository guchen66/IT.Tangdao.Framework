using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FakeDataInfoAttribute : Attribute
    {
        private string _description;

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>
        /// 用于指定数据类型
        /// </summary>
        private Type _datatype;

        public Type DataType
        {
            get => _datatype;
            set => _datatype = value;
        }

        /// <summary>
        /// 用来指定输出长度
        /// </summary>
        private int _length;

        public int Length
        {
            get => _length;
            set => _length = value;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        private string _defaultValue;

        public string DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        /// <summary>
        /// 用于指定bool值是否随机
        /// </summary>
        private bool _random;

        public bool Random
        {
            get => _random;
            set => _random = value;
        }
    }
}