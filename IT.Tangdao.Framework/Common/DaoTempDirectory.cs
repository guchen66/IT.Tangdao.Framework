using IT.Tangdao.Framework.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommon
{
    /// <summary>
    /// 临时文件的创建与移除
    /// </summary>
    public sealed class DaoTempDirectory : IDisposable
    {
        private static readonly IDaoLogger Logger = DaoLogger.Get(typeof(DaoTempDirectory));

        public string Path { get; }

        public DaoTempDirectory(string tempDirectoryPath)
        {
            Path = tempDirectoryPath;

            // Clean up the path, in order to make sure the folder is empty.
            DeleteFileSystemEntry(Path);

            Directory.CreateDirectory(Path);
        }

        public bool MoveTo(string targetPath, string subDirectoryName = null)
        {
            try
            {
                var directory = System.IO.Path.Combine(Path, subDirectoryName ?? string.Empty);
                if (!Directory.Exists(directory))
                    throw new DirectoryNotFoundException();

                Directory.Move(directory, targetPath);

                return true;
            }
            catch (Exception e)
            {
                Logger.Error("An unexpected exception occurred. ", e);

                return false;
            }
        }

        public static void DeleteFileSystemEntry(string path)
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            if (File.Exists(path)) File.Delete(path);
        }

        public void Dispose()
        {
            DeleteFileSystemEntry(Path);
        }
    }
}
