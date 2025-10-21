using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public static class TangdaoOperable
    {
        // 1. 值为 null 时变成 None
        public static TangdaoOptional<T> NotNullable<T>(T value) =>
            TangdaoOptional<T>.Some(value);
    }
}