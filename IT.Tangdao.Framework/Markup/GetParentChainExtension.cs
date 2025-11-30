using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace IT.Tangdao.Framework.Markup
{
    /// <summary>
    /// 视觉树父级链获取扩展
    /// 用于获取当前控件在视觉树中的所有父级元素，支持类型过滤和深度限制
    /// </summary>
    [MarkupExtensionReturnType(typeof(List<DependencyObject>))]
    public class GetParentChainExtension : MarkupExtension
    {
        /// <summary>
        /// 要查找的父级元素类型
        /// 如果为null，则返回所有父级元素
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        /// 最大查找深度，默认值为10
        /// 防止无限循环和性能问题
        /// </summary>
        public int MaxDepth { get; set; } = 10;

        /// <summary>
        /// 是否支持类型继承
        /// 如果为true，则返回所有继承自ParentType的父级元素
        /// 如果为false，则只返回类型完全匹配的父级元素
        /// </summary>
        public bool SupportInheritance { get; set; } = true;

        /// <summary>
        /// 是否同时搜索逻辑树
        /// 如果为true，当视觉树中找不到父级时，会尝试从逻辑树中查找
        /// </summary>
        public bool IncludeLogicalTree { get; set; } = false;

        /// <summary>
        /// 提供扩展的值
        /// </summary>
        /// <param name="serviceProvider">服务提供程序，用于获取XAML解析服务</param>
        /// <returns>符合条件的父级元素列表，如果没有找到则返回null</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 获取绑定的目标对象
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            // 检查目标对象是否为DependencyObject
            if (provideValueTarget?.TargetObject is DependencyObject current)
            {
                var parents = new List<DependencyObject>();
                var depth = 0;

                // 遍历视觉树，查找父级元素
                while (current != null && depth < MaxDepth)
                {
                    // 从视觉树中获取父级
                    var parent = VisualTreeHelper.GetParent(current);

                    // 如果视觉树中没有父级，且启用了逻辑树搜索
                    if (parent == null && IncludeLogicalTree)
                    {
                        // 从逻辑树中获取父级
                        parent = LogicalTreeHelper.GetParent(current);
                    }

                    // 如果找到了父级
                    if (parent != null)
                    {
                        // 检查是否符合类型条件
                        if (IsMatchType(parent))
                        {
                            parents.Add(parent);
                        }

                        // 继续查找下一级父级
                        current = parent;
                    }
                    else
                    {
                        // 没有找到父级，退出循环
                        break;
                    }

                    depth++;
                }

                return parents;
            }

            // 目标对象不是DependencyObject，返回null
            return null;
        }

        /// <summary>
        /// 检查父级元素是否匹配指定类型
        /// </summary>
        /// <param name="parent">要检查的父级元素</param>
        /// <returns>如果匹配则返回true，否则返回false</returns>
        private bool IsMatchType(DependencyObject parent)
        {
            // 如果没有指定类型，则匹配所有父级
            if (ParentType == null)
            {
                return true;
            }

            var parentType = parent.GetType();

            // 根据SupportInheritance属性决定匹配方式
            if (SupportInheritance)
            {
                // 支持继承，使用IsAssignableFrom
                return ParentType.IsAssignableFrom(parentType);
            }
            else
            {
                // 不支持继承，使用完全匹配
                return ParentType == parentType;
            }
        }
    }
}