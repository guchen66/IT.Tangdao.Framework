namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoContainerBuilderExtension
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static void Singleton(this ITangdaoContainerBuilder container)
        {
            // ((TangdaoContainerBuilder)container).Singleton();
        }

        /// <summary>
        /// 瞬态模式
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static void Transient(this ITangdaoContainerBuilder container)
        {
            // ((TangdaoContainerBuilder)container).Transient();
        }

        /// <summary>
        /// 作用域模式
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static void Scoped(this ITangdaoContainerBuilder container)
        {
            // ((TangdaoContainerBuilder)container).Scoped();
        }
    }
}