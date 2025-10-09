using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    public readonly struct RelativePath : IEquatable<RelativePath>, IComparable<RelativePath>
    {
        private readonly string _path;

        public RelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                _path = string.Empty;
                return;
            }
            _path = NormalizeSeparators(path);
        }

        /// <summary>
        /// 把 / 和 \ 统一成当前平台分隔符，只 new 一次字符串
        /// </summary>
        private static string NormalizeSeparators(string src)
        {
            char platSep = Path.DirectorySeparatorChar;
            char altSep = platSep == '/' ? '\\' : '/';

            // 常见场景：没有替代分隔符，直接返回原串
            if (src.IndexOf(altSep) == -1)
                return src;

            char[] buf = src.ToCharArray();
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] == altSep)
                    buf[i] = platSep;
            return new string(buf);
        }

        public string Value => _path;
        public static RelativePath Empty { get; } = new RelativePath(string.Empty);

        // 重写 ToString() 和相等性方法
        public override string ToString() => _path;

        public bool Equals(RelativePath other) =>
            string.Equals(_path, other._path, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) =>
            obj is RelativePath other && Equals(other);

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_path);

        // 操作符重载
        public static bool operator ==(RelativePath left, RelativePath right) => left.Equals(right);

        public static bool operator !=(RelativePath left, RelativePath right) => !left.Equals(right);

        // 转换操作符
        public static explicit operator string(RelativePath path) => path._path;

        // 从字符串隐式转换（方便创建）
        public static implicit operator RelativePath(string path) => new RelativePath(path);

        // 路径操作方法
        public RelativePath Combine(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return this;

            string newPath = Path.Combine(_path, relativePath);
            return new RelativePath(newPath);
        }

        public RelativePath WithExtension(string extension)
        {
            string newPath = Path.ChangeExtension(_path, extension);
            return new RelativePath(newPath);
        }

        public RelativePath Parent()
        {
            string parent = Path.GetDirectoryName(_path);
            return new RelativePath(parent ?? _path);
        }

        // 获取文件名部分
        public string FileName => Path.GetFileName(_path);

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(_path);

        // 判断是否是当前目录
        public bool IsCurrentDirectory => _path == "." || string.IsNullOrEmpty(_path);

        // 判断是否是上级目录引用
        public bool StartsWithParentReference =>
            _path?.StartsWith("..") == true ||
            (_path?.Contains($"..{Path.DirectorySeparatorChar}") == true);

        public int CompareTo(RelativePath other) =>
        StringComparer.OrdinalIgnoreCase.Compare(_path, other._path);
    }
}