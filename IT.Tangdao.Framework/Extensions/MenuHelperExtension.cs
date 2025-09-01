using IT.Tangdao.Framework.DaoParameters.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class MenuHelperExtension
    {
        public static ITangdaoMenuItem Find(this ITangdaoMenuItem menuItem, string key, bool create = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return menuItem;
            }

            string[] pathSegments = key.Split('/'); // 假设用/分隔路径
            ITangdaoMenuItem current = menuItem;

            foreach (string segment in pathSegments)
            {
                if (string.IsNullOrWhiteSpace(segment)) continue;

                var child = current.Childs?.FirstOrDefault(x => x.MenuName.Equals(segment, StringComparison.OrdinalIgnoreCase));

                if (child == null && create)
                {
                    child = new TangdaoMenuItem
                    {
                        MenuName = segment,
                        Childs = new List<ITangdaoMenuItem>()
                    };
                    if (current.Childs == null)
                    {
                        current.Childs = new List<ITangdaoMenuItem>();
                    }
                    current.Childs.Add(child);
                }

                if (child == null) return null;

                current = child;
            }

            return current;
        }
    }
}