using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Enums;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IQueryFormattable
    {
        //  string Format(DaoFileType daoFileType, IQueryFormatProvider formatProvider);
    }

    //public class QueryFormattable : IQueryFormattable
    //{
    //    public string Format(DaoFileType daoFileType, IQueryFormatProvider formatProvider)
    //    {
    //       return formatProvider.Revolse(daoFileType);
    //    }
    //}
    //public interface IQueryFormatProvider
    //{
    //    string Revolse(DaoFileType daoFileType);
    //}

    //public class QueryFormatProvider : IQueryFormatProvider
    //{
    //    public string Revolse(DaoFileType daoFileType)
    //    {
    //        return "."+daoFileType.ToString().ToLowerInvariant();
    //    }
    //}
    //public interface IContentXmlQueryable
    //{
    //    IContentXmlQueryable SelectNode();
    //}

    //public class ContentXmlQueryable : IContentXmlQueryable
    //{
    //    public IContentXmlQueryable SelectNode()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public interface IContentJsonQueryable
    //{
    //    IContentJsonQueryable SelectValue(string key);
    //}

    //public class ContentJsonQueryable : IContentJsonQueryable
    //{
    //    public IContentJsonQueryable SelectValue(string key)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public interface IContentConfigQueryable
    //{
    //    IContentJsonQueryable SelectConfig(string key);
    //}

    //public class ContentConfigQueryable : IContentConfigQueryable
    //{
    //    public IContentJsonQueryable SelectConfig(string key)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public interface IContentXmlQueryable : IContentQueryable
    //{
    //    Task<string> SelectNode(string xpath);
    //    Task<T> Select<T>(string xpath = null);
    //}

    //public interface IContentJsonQueryable : IContentQueryable
    //{
    //    Task<string> SelectValue(string key);
    //    Task<T> Select<T>(string key = null);
    //}

    //public interface IContentConfigQueryable : IContentQueryable
    //{
    //    Task<string> SelectConfig(string section);
    //}
}