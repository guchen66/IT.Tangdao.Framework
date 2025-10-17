using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class DateTimeAwaitExtension
    {
        public static TaskAwaiter GetAwaiter(this DateTime dateTime)
        {
            var delay = dateTime - DateTime.Now;
            return Task.Delay(delay).GetAwaiter();
        }
    }
}