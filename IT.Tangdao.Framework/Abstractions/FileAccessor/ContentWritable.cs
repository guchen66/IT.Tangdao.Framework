using IT.Tangdao.Framework.Abstractions.Results;
using System;
using System.IO;
using System.Linq;
using IT.Tangdao.Framework.Helpers;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public sealed class ContentWritable : IContentWritable
    {
        public string Content { get; set; }

        public string WritePath { get; }

        public DaoFileType DetectedType { get; }
    }
}