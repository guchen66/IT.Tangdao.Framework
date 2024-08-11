using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.DaoDtos.Options;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoProvider: ITangdaoProvider
    {
        private readonly List<CommonContext> _contexts;

        public TangdaoProvider(ITangdaoAdapter adapter)
        {
            _contexts = adapter.GetContexts();
        }

        public List<CommonContext> GetContexts()
        {
            return _contexts;
        }

        /// <summary>
        /// 解析的时候无非分为三种
        /// 1、解析的是接口
        /// 2、解析的是具有无参构造的类
        /// 3、解析的是具有有参构造的类
        /// 
        /// 所以当它具有无参构造器，我不需要注册，直接解析
        /// 当它具有有参构造器的时候，我在从字典里捞，并且递归
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Resolve(Type type)
        {
            if (type.IsInterface)
            {
                ResolveInterface(type);
            }
            else if(type.IsClass && type.GetConstructor(Type.EmptyTypes) != null)
            {
               return Activator.CreateInstance(type);
            }
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            var context = _contexts.LastOrDefault();
            var serviceType=context.RegisterType[type] as Type;
            List<object> parameList=new List<object>();
            if (constructorInfos.Length > 0)
            {
                ConstructorInfo constructorInfo= constructorInfos[0];
                foreach (ParameterInfo parameter in constructorInfo.GetParameters())
                {
                    Type paramType= parameter.ParameterType;
                    if (!context.RegisterType.ContainsKey(paramType))
                    {
                        TangdaoGuards.ThrowIfNull($"{paramType}，为空，未注册");
                        throw new InvalidOperationException();
                    }

                    //获取类的映射
                    //  Type implementationType= paramType.GetGenericTypeDefinition();
                    var implementationType = context.RegisterType[paramType] as Type;
                    var instance = Resolve(implementationType);
                    parameList.Add(instance);
                }
            }
          
            var obj=Activator.CreateInstance(serviceType, parameList.ToArray());
            return obj;
        }

        public object ResolveClass(Type type)
        {
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            var context = _contexts.LastOrDefault();
            List<object> parameList = new List<object>();
            if (constructorInfos.Length > 0)
            {
                ConstructorInfo constructorInfo = constructorInfos[0];
                foreach (ParameterInfo parameter in constructorInfo.GetParameters())
                {
                    Type paramType = parameter.ParameterType;
                    if (!context.RegisterType.ContainsKey(paramType))
                    {
                        TangdaoGuards.ThrowIfNull($"{paramType}，为空，未注册");
                        throw new InvalidOperationException();
                    }

                    //获取类的映射
                    //  Type implementationType= paramType.GetGenericTypeDefinition();
                    var implementationType = context.RegisterType[paramType] as Type;
                    var instance = Resolve(implementationType);
                    parameList.Add(instance);
                }
            }

            var obj = Activator.CreateInstance(type, parameList.ToArray());
            return obj;
        }

        public object ResolveInterface(Type type)
        {
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            var context = _contexts.LastOrDefault();
            var serviceType = context.RegisterType[type] as Type;
            List<object> parameList = new List<object>();
            if (constructorInfos.Length > 0)
            {
                ConstructorInfo constructorInfo = constructorInfos[0];
                foreach (ParameterInfo parameter in constructorInfo.GetParameters())
                {
                    Type paramType = parameter.ParameterType;
                    if (!context.RegisterType.ContainsKey(paramType))
                    {
                        TangdaoGuards.ThrowIfNull($"{paramType}，为空，未注册");
                        throw new InvalidOperationException();
                    }

                    //获取类的映射
                    //  Type implementationType= paramType.GetGenericTypeDefinition();
                    var implementationType = context.RegisterType[paramType] as Type;
                    var instance = ResolveInterface(implementationType);
                    parameList.Add(instance);
                }
            }

            var obj = Activator.CreateInstance(serviceType, parameList.ToArray());
            return obj;
        }
        public object Resolve(Type type, bool useFactoryMethods = false)
        {
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            var context = _contexts.LastOrDefault();
            List<object> parameList = new List<object>();

            if (constructorInfos.Length > 0)
            {
                ConstructorInfo constructorInfo = constructorInfos[0];
                foreach (ParameterInfo parameter in constructorInfo.GetParameters())
                {
                    Type paramType = parameter.ParameterType;
                    if (!context.RegisterType.ContainsKey(paramType))
                    {
                        TangdaoGuards.ThrowIfNull($"{paramType}，为空，未注册");
                        throw new InvalidOperationException();
                    }

                    if (useFactoryMethods)
                    {
                        var factoryMethod = context.RegisterType[paramType] as Func<ITangdaoProvider, object>;
                        if (factoryMethod != null)
                        {
                            parameList.Add(factoryMethod(this));
                        }
                        else
                        {
                            var creator = context.RegisterType[paramType] as Func<object>;
                            if (creator != null)
                            {
                                parameList.Add(creator());
                            }
                            else
                            {
                                var implementationType = context.RegisterType[paramType] as Type;
                                var instance = Resolve(implementationType); // 递归解析依赖项
                                parameList.Add(instance);
                            }
                        }
                    }
                    else
                    {
                        var implementationType = context.RegisterType[paramType] as Type;
                        var instance = Resolve(implementationType); // 递归解析依赖项
                        parameList.Add(instance);
                    }
                }
            }

            var obj = Activator.CreateInstance(type, parameList.ToArray());
            return obj;
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
