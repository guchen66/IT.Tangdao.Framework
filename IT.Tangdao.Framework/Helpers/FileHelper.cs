using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 判断文件的后缀
    /// </summary>
    public class FileHelper
    {
        public bool Root { get; set; }

        public static string GetFileType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            // 获取文件扩展名（包括点）
            string extension = Path.GetExtension(filePath);

            // 根据扩展名判断文件类型
            switch (extension.ToLower())
            {
                case ".xaml":
                    return "XAML";

                case ".cs":
                    return "CS";

                default:
                    return "Unknown";
            }
        }
    }
}