using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 目录文件帮助类
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 获取根目录
        /// </summary>
        /// <returns></returns>
        public static string SelectRootDirectory()
        {
            string rootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            return rootDirectory;
        }

        /// <summary>
        /// 递归搜索指定文件，优先当前目录，其次子目录（广度优先）
        /// </summary>
        /// <param name="fileName">目标文件名（如：appsettings.json）</param>
        /// <param name="rootDir">起始搜索目录（默认当前目录）</param>
        /// <returns>找到的第一个文件完整路径，未找到时返回null</returns>
        public static string SelectDirectoryByName(string fileName, string rootDir = null)
        {
            if (rootDir == null)
            {
                rootDir = Directory.GetCurrentDirectory();
            }

            // 优先检查当前目录
            var directPath = Path.Combine(rootDir, fileName);
            if (File.Exists(directPath))
            {
                return directPath;
            }

            // 广度优先搜索子目录
            var dirsToSearch = new Queue<string>();
            dirsToSearch.Enqueue(rootDir);

            while (dirsToSearch.Count > 0)
            {
                var currentDir = dirsToSearch.Dequeue();

                try
                {
                    // 检查当前目录文件
                    var filePath = Path.Combine(currentDir, fileName);
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }

                    // 将子目录加入队列
                    foreach (var subDir in Directory.GetDirectories(currentDir))
                    {
                        dirsToSearch.Enqueue(subDir);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // 跳过无权限访问的目录
                    continue;
                }
            }

            return null; // 未找到
        }

        /// <summary>
        /// 获取指定类库，指定某个文件夹下，并且应用了某个特性的所有类
        /// </summary>
        public static Type[] GetClassSelf(string lib, string folder, Type inter = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentException("文件夹不存在", nameof(folder));
            }

            try
            {
                var classTypes = GetTypesInfoByLinq(lib, folder, inter);

                return classTypes.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取类失败：{ex.Message}");
                return new Type[0]; // 返回空数组或者抛出异常，根据实际需求进行处理
            }
        }

        private static IEnumerable<Type> GetTypesInfoByLinq(string lib, string folder, Type inter)
        {
            // 加载指定的程序集
            Assembly callingAssembly = Assembly.Load(lib);
            // 获取程序集中所有的类型
            Type[] allTypes = callingAssembly.GetTypes();
            // 筛选出位于指定文件夹下，并且应用了指定特性的所有类
            IEnumerable<Type> modelTypes = allTypes
                .Where(type => type.Namespace?.Contains(folder) == true && (inter == null || Attribute.IsDefined(type, inter)));

            return modelTypes;
        }

        /// <summary>
        /// 获取解决方案目录
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionPath()
        {
            string SolutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            return SolutionPath;
        }

        /// <summary>
        /// 获取解决方案Name
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionName()
        {
            string SolutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Name;
            return SolutionPath;
        }

        /// <summary>
        /// 获取主程序所在目录
        /// </summary>
        /// <returns></returns>
        public static string GetMainProgramPath()
        {
            string MainProgramPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return MainProgramPath;
        }
    }
}