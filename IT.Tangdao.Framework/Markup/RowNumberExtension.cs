using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 对IItemsControl做自增Id选项
    /// </summary>
    public class RowNumberExtension : MarkupExtension
    {
        public int StartIndex { get; set; } = 1;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            // 检查目标是否为可绑定的列或元素
            if (IsBindableTarget(provideValueTarget?.TargetObject))
            {
                var multiBinding = new MultiBinding();

                // 绑定到当前数据项
                multiBinding.Bindings.Add(new Binding());

                // 绑定到最近的 ItemsControl 祖先
                multiBinding.Bindings.Add(new Binding
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ItemsControl), 1)
                });

                multiBinding.Converter = new RowNumberMultiConverter { StartIndex = StartIndex };
                return multiBinding;
            }

            return StartIndex;
        }

        private bool IsBindableTarget(object target)
        {
            // 支持 DataGridTextColumn
            if (target is DataGridTextColumn)
                return true;

            // 支持 GridViewColumn
            if (target is GridViewColumn)
                return true;

            // 支持直接绑定到元素的 Text 属性
            if (target is TextBlock)
                return true;

            // 可以根据需要扩展其他可绑定目标
            return false;
        }

        private class RowNumberMultiConverter : IMultiValueConverter
        {
            public int StartIndex { get; set; } = 1;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length == 2)
                {
                    var item = values[0];
                    if (item != null && values[1] is ItemsControl itemsControl)
                    {
                        // 方法1：直接遍历 Items 获取索引（最可靠）
                        int index = 0;
                        foreach (var i in itemsControl.Items)
                        {
                            if (Equals(i, item))
                            {
                                return (index + StartIndex).ToString();
                            }
                            index++;
                        }

                        // 方法2：尝试通过 ItemContainerGenerator 获取（备选方案）
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                        if (container != null)
                        {
                            int containerIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(container);
                            if (containerIndex >= 0)
                            {
                                return (containerIndex + StartIndex).ToString();
                            }
                        }
                    }
                }
                return "";
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    }
}