using IT.Tangdao.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Paths;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.Collections
{
    /// <summary>
    /// 临时文件的创建与移除
    /// </summary>
    public sealed class TempDirectory : IDisposable
    {
        public AbsolutePath Path { get; }

        public bool IsAutoDelete { get; }

        // 核心构造函数：接受 AbsolutePath
        public TempDirectory(AbsolutePath tempDirectoryPath, bool isAutoDelete)
        {
            Path = tempDirectoryPath;
            IsAutoDelete = isAutoDelete;
            DeleteFileSystemEntry(Path.Value);
            Directory.CreateDirectory(Path.Value);
        }

        // 便利构造函数：接受 string，内部转换为 AbsolutePath
        public TempDirectory(string tempDirectoryPath, bool isAutoDelete) : this(new AbsolutePath(tempDirectoryPath), isAutoDelete)
        {
        }

        public ResponseResult MoveTo(AbsolutePath targetPath, string subDirectoryName = null)
        {
            try
            {
                var directory = subDirectoryName.IsNullOrEmptyToken()
                    ? Path.Value
                    : System.IO.Path.Combine(Path.Value, subDirectoryName);

                Directory.Move(directory, targetPath.Value);
                return ResponseResult.Success(message: "移动完成", value: directory);
            }
            catch (Exception e)
            {
                return ResponseResult.FromException(e);
            }
        }

        // 保留 string 重载方便调用
        public ResponseResult MoveTo(string targetPath, string subDirectoryName = null)
            => MoveTo(new AbsolutePath(targetPath), subDirectoryName);

        internal static void DeleteFileSystemEntry(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            if (File.Exists(path)) File.Delete(path);
        }

        internal static void DeleteFileSystemEntry(AbsolutePath path)
            => DeleteFileSystemEntry(path.Value);

        public void Dispose()
        {
            if (IsAutoDelete)
            {
                DeleteFileSystemEntry(Path.Value);
            }
        }
    }
}