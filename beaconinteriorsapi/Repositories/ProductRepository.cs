using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Utils;
using Microsoft.EntityFrameworkCore;

namespace beaconinteriorsapi.Repositories
{
    public class ProductRepository(BeaconInteriorsDBContext dbContext)
    {
        private readonly BeaconInteriorsDBContext _dbContext=dbContext;
        public async Task<IEnumerable<Product>> GetProductsPaginatedAsync(int page,int pageSize)
        {
                return await _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).AsQueryable().Paginate(page,pageSize).ToListAsync(); 

        }
        public async Task<Product?> GetByIdAsync(Guid productId)
        {
                return await _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).FirstOrDefaultAsync(p => p.Id == productId);

        }
        public async Task<IEnumerable<Product>> SearchByCategoryAndNameAsync(ProductSearchParams searchParams)
        {
            var query = _dbContext.Products
                .AsQueryable().Include(p => p.Images)
                .Include(p => p.Categories).AsQueryable();
            if (!string.IsNullOrEmpty(searchParams.Name))
                query=query.Where(p => p.Name.ToLower().Contains(searchParams.Name.Trim().ToLower()));
            if (!string.IsNullOrEmpty(searchParams.Category))
                query=query.Where(p => p.Categories.Any(c => c.Name.ToLower() == searchParams.Category.Trim().ToLower()));
            return await query.ToListAsync();
        }
        public async Task CreateAsync(Product product)
        {
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();   
        }
        public async Task<bool> DeleteAsync(Guid productId)
        {
                var product = await _dbContext.Products
                    .Include(p => p.Images)
                    .Include(p => p.Categories)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    return false;
                }

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                return true;
        }

    }
}
