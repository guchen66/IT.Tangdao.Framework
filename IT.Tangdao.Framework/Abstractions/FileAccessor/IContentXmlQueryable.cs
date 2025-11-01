using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentXmlQueryable : IContentQueryable
    {
        ResponseResult SelectNode(string node);

        ResponseResult SelectNodes();

        ResponseResult<List<T>> SelectNodes<T>(string rootElement, Func<XElement, T> selector);

        ResponseResult<List<T>> SelectNodes<T>() where T : new();
    }
}