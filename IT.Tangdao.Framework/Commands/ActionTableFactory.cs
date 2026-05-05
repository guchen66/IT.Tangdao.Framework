using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 工厂模式
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ActionTableFactory
    {
        private static readonly Lazy<IActionTable> _instance =
            new Lazy<IActionTable>(() => new ActionTable());

        public static IActionTable GetInstance() => _instance.Value;
    }
}