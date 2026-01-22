using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Utils;
using Microsoft.EntityFrameworkCore;

namespace beaconinteriorsapi.Repositories
{
    public class ProductRepository(BeaconInteriorsDBContext dbContext)
    {
        private readonly BeaconInteriorsDBContext _dbContext=dbContext;
        public async Task<IEnumerable<Product>> GetProductsPaginatedAsync(int page,int pageSize,string? name,List<string>category)
        {
            var query = _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).AsQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(p => p.Name.ToLower().Contains(name.Trim().ToLower()));
            if (category.Count > 0)
            {
                foreach (var cat in category)
                {
                    query = query.Where(p => p.Categories.Any(c => c.Name.ToLower() == cat.Trim().ToLower()));
                }
            }
            return await query.Paginate(page,pageSize).ToListAsync(); 

        }
        public async Task<Product?> GetByIdAsync(Guid productId)
        {
                return await _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).FirstOrDefaultAsync(p => p.Id == productId);

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
