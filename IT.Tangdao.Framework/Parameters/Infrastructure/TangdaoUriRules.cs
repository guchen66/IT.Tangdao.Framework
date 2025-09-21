using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Parameters.Infrastructure
{
    /// <summary>
    /// WPF 约定式 Pack URI 工厂：只要给短名，返回真实 Pack URI
    /// </summary>
    public static class TangdaoUriRules
    {
        /// <summary>
        /// 短名→Pack URI（自动补 .xaml）
        /// </summary>
        /// <param name="shortName">例如 "Login" 或 "Views/Login"</param>
        /// <param name="folder">约定文件夹：Views / ViewModels / Resources</param>
        /// <returns>Pack URI</returns>
        public static Uri ToPackUri(string shortName, ConventionalFolder folder = ConventionalFolder.Views)
        {
            if (shortName == null) throw new ArgumentNullException(nameof(shortName));

            // 1. 统一后缀
            if (!shortName.EndsWithIgnoreCase(".xaml"))
                shortName += ".xaml";

            // 2. 拼相对路径
            string relative;
            switch (folder)
            {
                case ConventionalFolder.Views:
                    relative = $"Views/{shortName}";
                    break;

                case ConventionalFolder.ViewModels:
                    relative = $"ViewModels/{shortName}";
                    break;

                case ConventionalFolder.Resources:
                    relative = $"Resources/{shortName}";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(folder));
            }

            // 3. 当前程序集 Pack URI
            string pack = $"/{Application.Current?.GetType().Assembly.GetName().Name};component/{relative}";
            // 显式带协议头，并且用 Absolute
            return new Uri($"pack://application:,,,{pack}", UriKind.Absolute);
        }

        //public static string ViewToViewModel(string viewName)
        //{
        //}

        // public static string ViewModelToView(string viewModelName)
        // {
        // }
    }
}