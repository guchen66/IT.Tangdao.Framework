using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Selectors
{
    public class TangdaoAttributeSelector
    {
        //private readonly List<TSource> _source;
        //private Func<TSource, bool> _predicate;

        //public TangdaoAttributeSelector(IEnumerable<TSource> source)
        //{
        //    _source = source.ToList();
        //}

        //// 静态方法用于开始查询
        //public static TangdaoAttributeSelector<TSource> Select<TSource>(IEnumerable<TSource> source)
        //{
        //    return new TangdaoAttributeSelector<TSource>(source);
        //}

        //// Select 方法，用于过滤源数据
        //public TangdaoAttributeSelector<TSource> Where(Func<TSource, bool> predicate)
        //{
        //    _predicate = predicate;
        //    return this;
        //}

        //// 获取结果的方法
        //public IEnumerable<TSource> ToList()
        //{
        //    if (_predicate == null)
        //        return _source;

        //    return _source.Where(_predicate);
        //}

        // 辅助类，用于存储选择的信息
        public class SelectInfo
        {
            public string TypeName { get; set; }
            public string Sign { get; set; }
        }

        public static TSource Select1<TSource>(Func<TSource, bool> value) where TSource : TangdaoAttribute
        {
            throw new NotImplementedException();
        }
    }
}