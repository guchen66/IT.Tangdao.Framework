using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 为所有数字类型提供扩展的方法
    /// </summary>
    public static class NumericExtension
    {
        public static int Square(this int value) => (int)SquareCore(value);

        public static double Square(this double value) => SquareCore(value);

        //public static decimal Square(this decimal value) => SquareCore(value);

        private static double SquareCore(double value) => value * value;
    }
}