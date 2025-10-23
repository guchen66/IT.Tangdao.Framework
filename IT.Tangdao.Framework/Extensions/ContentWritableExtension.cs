using IT.Tangdao.Framework.Abstractions.FileAccessor;
using IT.Tangdao.Framework.Abstractions;

namespace IT.Tangdao.Framework.Extensions
{
    internal static class ContentWritableExtension
    {
        public static void Serialize(this IContentWritable contentWriteable)
        {
            contentWriteable.Serialize();
        }
    }
}