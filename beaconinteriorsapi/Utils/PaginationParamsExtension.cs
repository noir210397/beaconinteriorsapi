using beaconinteriorsapi.Models;

namespace beaconinteriorsapi.Utils
{
    public static class PaginationParamsExtensions
    {
        public static (int Page, int PageSize) Normalize(this PaginationParams pagination)
        {
            var page = pagination.Page <= 0 ? 1 : pagination.Page;
            var pageSize = pagination.PageSize <= 0 ? 20 : Math.Min(pagination.PageSize, 20);
            return (page, pageSize);
        }
    }

}
