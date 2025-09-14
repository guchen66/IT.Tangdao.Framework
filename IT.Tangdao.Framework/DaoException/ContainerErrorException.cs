using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 唐刀容器（IoC）阶段错误：注册失败、解析失败、生命周期冲突等。
    /// </summary>
    [Serializable]
    public class ContainerErrorException : Exception
    {
        /// <summary>
        /// 出错阶段
        /// </summary>
        public ContainerPhase Phase { get; }

        /// <summary>
        /// 涉及的服务类型
        /// </summary>
        public Type ServiceType { get; }

        public enum ContainerPhase
        {
            Register,
            Resolve,
            ScopeBuild,
            SingletonCheck
        }

        #region 标准构造器

        public ContainerErrorException()
        { }

        public ContainerErrorException(string message) : base(message)
        {
        }

        public ContainerErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion 标准构造器

        #region 专用工厂

        /// <summary>
        /// 注册阶段出错
        /// </summary>
        public static ContainerErrorException OnRegister(Type serviceType, string reason, Exception inner = null)
            => new ContainerErrorException(
                $"注册服务 '{serviceType.FullName}' 失败：{reason}",
                ContainerPhase.Register, serviceType, inner);

        /// <summary>
        /// 解析阶段出错
        /// </summary>
        public static ContainerErrorException OnResolve(Type serviceType, string reason, Exception inner = null)
            => new ContainerErrorException(
                $"解析服务 '{serviceType.FullName}' 失败：{reason}",
                ContainerPhase.Resolve, serviceType, inner);

        #endregion 专用工厂

        #region 完整构造器

        public ContainerErrorException(string message, ContainerPhase phase, Type serviceType, Exception inner = null)
            : base(message, inner)
        {
            Phase = phase;
            ServiceType = serviceType;
        }

        #endregion 完整构造器

        #region 序列化支持

#if NET8_0_OR_GREATER
        [Obsolete("BinaryFormatter is obsolete in .NET 8+")]
#endif

        protected ContainerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Phase = (ContainerPhase)info.GetInt32(nameof(Phase));
            ServiceType = Type.GetType(info.GetString(nameof(ServiceType)) ?? string.Empty);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Phase), (int)Phase);
            info.AddValue(nameof(ServiceType), ServiceType?.AssemblyQualifiedName);
        }

        #endregion 序列化支持
    }
}