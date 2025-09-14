using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 属性值未通过框架校验规则。
    /// </summary>
    public class PropertyValidationException : TangdaoException
    {
        public string PropertyName { get; }

        public PropertyValidationException(string propertyName, string message) : base($"属性 '{propertyName}' 校验失败：{message}")
        {
            PropertyName = propertyName;
        }

        public PropertyValidationException(string propertyName, string message, Exception inner) : base($"属性 '{propertyName}' 校验失败：{message}", inner)
        {
            PropertyName = propertyName;
        }
    }
}