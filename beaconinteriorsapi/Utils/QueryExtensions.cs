namespace beaconinteriorsapi.Utils
{
    public static class QueryExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return query.Skip(skip).Take(pageSize);
        }
    }
}
