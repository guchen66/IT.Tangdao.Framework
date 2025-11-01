using IT.Tangdao.Framework.DaoException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    /// <summary>
    /// 零依赖路径模板 - 仿 NodaTime LocalDatePattern
    /// </summary>
    public sealed class PathTemplate
    {
        private readonly Token[] _tokens;
        private readonly string _rawTemplate;

        private PathTemplate(string rawTemplate, Token[] tokens)
        {
            _rawTemplate = rawTemplate;
            _tokens = tokens;
        }

        /// <summary>
        /// 解析模板，只做一次
        /// </summary>
        public static PathTemplate Create(string template)
        {
            if (string.IsNullOrEmpty(template))
                throw new ArgumentException("Template cannot be null or empty.", nameof(template));

            var tokens = Parse(template);
            return new PathTemplate(template, tokens);
        }

        /*---------- 解析 ----------*/

        private static Token[] Parse(string template)
        {
            var list = new List<Token>();
            int i = 0;
            while (i < template.Length)
            {
                if (template[i] == '{')
                {
                    int close = template.IndexOf('}', i + 1);
                    if (close == -1)
                        throw new PathSyntaxException(template, "缺少闭合 '}'。");
                    string key = template.Substring(i + 1, close - i - 1);
                    if (string.IsNullOrWhiteSpace(key))
                        throw new PathSyntaxException(template, "占位符不能为空。");
                    list.Add(new Token(TokenType.Placeholder, key));
                    i = close + 1;
                }
                else
                {
                    int next = template.IndexOf('{', i);
                    string literal = next == -1 ? template.Substring(i) : template.Substring(i, next - i);
                    list.Add(new Token(TokenType.Literal, literal));
                    i = next == -1 ? template.Length : next;
                }
            }
            return list.ToArray();
        }

        /*---------- Resolve 重载 ----------*/

        public AbsolutePath Resolve(object anonymous)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in anonymous.GetType().GetProperties())
                dict[prop.Name] = prop.GetValue(anonymous, null)?.ToString() ?? string.Empty;
            return Resolve(dict);
        }

        public AbsolutePath Resolve(IDictionary<string, string> values)
        {
            var sb = new StringBuilder();
            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.Literal)
                    sb.Append(token.Text);
                else
                {
                    if (!values.TryGetValue(token.Text, out var val))
                        throw new KeyNotFoundException($"模板缺少键 '{token.Text}'。");
                    sb.Append(NormalizeSegment(val));
                }
            }
            return new AbsolutePath(sb.ToString());
        }

        /*---------- 小工具 ----------*/

        // 放在类内部，只分配一次
        private static readonly char[] SeparatorsToNormalize = { '/', '\\' };

        private static string NormalizeSegment(string seg)
        {
            seg = seg.Trim();
            if (seg.IndexOfAny(SeparatorsToNormalize) == -1) return seg;

            char plat = Path.DirectorySeparatorChar;
            char alt = plat == '/' ? '\\' : '/';
            return seg.Replace(alt, plat);
        }

        /*---------- 内部 Token ----------*/

        private enum TokenType
        {
            Literal,
            Placeholder
        }

        private readonly struct Token
        {
            public readonly TokenType Type;
            public readonly string Text;

            public Token(TokenType type, string text)
            {
                Type = type;
                Text = text;
            }
        }
    }
}