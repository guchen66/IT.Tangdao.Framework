using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Navigates
{
    public static class NavigationContext
    {
        // AsyncLocal 保证 async/await 跨线程也能拿到
        private static readonly AsyncLocal<string> _slot = new AsyncLocal<string>();

        public static string CurrentGroup
        {
            get => _slot.Value;
            set => _slot.Value = value;
        }
    }
}