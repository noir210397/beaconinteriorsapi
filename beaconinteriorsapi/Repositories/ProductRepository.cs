using beaconinteriorsapi.Data;
using beaconinteriorsapi.Models;
using Microsoft.EntityFrameworkCore;

namespace beaconinteriorsapi.Repositories
{
    public class ProductRepository(BeaconInteriorsDBContext dbContext)
    {
        private readonly BeaconInteriorsDBContext _dbContext=dbContext;
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
                return await _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).ToListAsync(); 

        }
        public async Task<Product?> GetByIdAsync(Guid productId)
        {
                return await _dbContext.Products.Include(p => p.Images).Include(c => c.Categories).FirstOrDefaultAsync(p => p.Id == productId);

        }
        public async Task<IEnumerable<Product>> SearchByCategoryAsync(string categoryName)
        {
                return await _dbContext.Products
                    .Include(p => p.Images)
                    .Include(p => p.Categories)
                    .Where(p => p.Categories.Any(c => c.Name.ToLower() == categoryName.Trim().ToLower()))
                    .ToListAsync();
            
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
                return await _dbContext.Products
                    .Include(p => p.Images)
                    .Include(p => p.Categories)
                    .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                    .ToListAsync();

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
        public async Task UpdateAsync(Product product)
        {
            
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            
        }

    }
}
