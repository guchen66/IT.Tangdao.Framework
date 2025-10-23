﻿using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Helpers;
using System;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public class WriteService : IWriteService
    {
        public IContentWritable Default => new ContentWritable();
    }
}