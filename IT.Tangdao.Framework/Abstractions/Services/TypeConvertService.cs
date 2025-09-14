using IT.Tangdao.Framework.Abstractions.IServices;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.DaoException;
using System.Reflection;

namespace IT.Tangdao.Framework.Abstractions.Services
{
    public class TypeConvertService : ITypeConvertService
    {
        public T Converter<T>(string name) where T : class, new()
        {
            T type = new T();

            if (!typeof(T).IsHasConstructor())
            {
                throw new TypeErrorException("缺少无参构造器");
            }
            if (name != type.GetType().Name)
            {
                throw new ImproperNamingException($"{name}命名不规范");
            }
            return type;
        }
    }
}