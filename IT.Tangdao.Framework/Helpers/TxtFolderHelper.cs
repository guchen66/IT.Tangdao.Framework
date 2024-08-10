using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public class TxtFolderHelper
    {
        public static string SimpleRead(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);
        }

        public static string[] SimpleReadStringArray(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllLines(path);          
        }

        public static string ReadByFileStream(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static Task<string> SimpleReadAsync(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                return File.ReadAllText(path);
            });
        }
        public static Task<string[]> SimpleReadStringArrayAsync(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                return File.ReadAllLines(path);
            });
        }
        public static async Task<string> ReadByFileStreamAsync(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return await sr.ReadToEndAsync();
                }
            }
        }
    }
}
