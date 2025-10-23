namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    /// <inheritdoc/>
    public class ReadService : IReadService
    {
        public IContentQueryable Default => new ContentQueryable();

        public ICacheContentQueryable Cache => new CacheContentQueryable();
    }
}