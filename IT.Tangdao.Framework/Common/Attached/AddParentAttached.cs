using IT.Tangdao.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Common.Attached
{
    public static class AddParentAttached
    {
        // 让贴贴纸的 VM 直接拥有 “Parent” 依赖属性
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached(
                "Parent", typeof(object), typeof(AddParentAttached),
                new PropertyMetadata(null));

        public static void SetParent(IAddParent vm, object value) =>
            ((DependencyObject)vm).SetValue(ParentProperty, value);

        public static object GetParent(IAddParent vm) =>
            ((DependencyObject)vm).GetValue(ParentProperty);
    }
}