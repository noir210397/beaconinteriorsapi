using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;
using beaconinteriorsapi.Models;
using Microsoft.EntityFrameworkCore;

namespace beaconinteriorsapi.Services
{
    public class CategoryService
    {
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly IMapper _mapper;

        public CategoryService(BeaconInteriorsDBContext dbContext,IMapper mapper)
        {
          _dbContext=dbContext; 
           _mapper=mapper;
        }
        public async Task<CategoryDTO> AddCategoryAsync(string categoryName)
        {
            var categoryInDB=_dbContext.Categories.FirstOrDefault(c=>c.Name.ToLower()== categoryName.ToLower());
            if (categoryInDB != null)
            {
                ThrowBadRequest($"category by name {categoryName} already exists");
            }
            var category = new Category(categoryName.ToLower());
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category=_dbContext.Categories.FirstOrDefault(c=>c.Id==categoryId);
            if (category == null) { 
            ThrowNotFound($"category by Id:{categoryId} not found");
            }
            _dbContext.Categories.Remove(category!);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
                var categories=await _dbContext.Categories.ToListAsync();
                return _mapper.Map<IEnumerable<Category>,IEnumerable<CategoryDTO>>(categories);
        }
        public async Task<CategoryDTO> GetSingleCategoryAsync(int id)
        {
            var category= await _dbContext.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null) {
                ThrowNotFound($"no category found with Id:{id}");
            }
            return _mapper.Map<CategoryDTO>(category);

        }
        public async Task<CategoryDTO> UpdateCategoryAsync(int categoryId, string newCategoryValue)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                ThrowNotFound($"category by Id:{categoryId} not found");
            }
            category!.Name = newCategoryValue;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(category);
        }
    }
}
