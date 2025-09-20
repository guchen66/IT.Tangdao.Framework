using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 对Combobox进行定制列表
    /// </summary>
    public class OptionListExtension : MarkupExtension
    {
        // 1. 外部可注入的注册表
        public static IDictionary<string, IEnumerable> OptionsPool { get; } = new ConcurrentDictionary<string, IEnumerable>();

        // 2. 默认集合（Key = "Default"）
        static OptionListExtension()
        {
            OptionsPool["Default"] = new[] { "全部", "Load", "Upload" };
        }

        // 3. XAML 可调参数
        public string Key { get; set; } = "Default";   // 缺省用 Default

        public string Values { get; set; }             // 临时就地写列表
        public Binding Binding { get; set; }

        public override object ProvideValue(IServiceProvider sp)
        {
            // 优先级：Values > Binding > Key > Default
            if (!string.IsNullOrWhiteSpace(Values))
                return Values.Split(',').Select(s => s.Trim());

            if (Binding != null)              // 如果给了 Binding，直接返回它
                return Binding;               // ComboBox 会自己走 ItemsSource 绑定通路

            return OptionsPool.TryGetValue(Key, out var list) ? list : OptionsPool["Default"];
        }

        private sealed class NumericStringConverter : IValueConverter
        {
            public object Convert(object v, Type t, object p, CultureInfo c) => v?.ToString();

            public object ConvertBack(object v, Type t, object p, CultureInfo c)
                => double.TryParse(v?.ToString(), out var d) ? d : Binding.DoNothing;
        }
    }
}