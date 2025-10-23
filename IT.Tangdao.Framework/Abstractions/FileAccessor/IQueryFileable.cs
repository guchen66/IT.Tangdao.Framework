using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IQueryFileable : IEnumerable
    {
        Expression Expression { get; }

        Directory Directory { get; }

        File File { get; }
    }
}