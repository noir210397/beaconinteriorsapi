using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Repositories;
using Microsoft.EntityFrameworkCore;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly ProductRepository _repository;
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductService(
            ILogger<ProductService> logger,
            ProductRepository repository,
            IMapper mapper,
            BeaconInteriorsDBContext dbContext,IFileService fileService)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
           
        {
                _logger.LogInformation("Attempting to fetch all products in Database at {Date}",DateTime.Now);
                var products = await _repository.GetAllAsync();
                if (products == null || !products.Any())
                {
                    _logger.LogWarning("no products found in database");
                    ThrowNotFound("No products found");
                }
                return _mapper.Map<IEnumerable<Product>, IList<ProductDTO>>(products!);
        }
        public async Task<ProductDTO> GetSingleProductAsync(Guid productId)
        { 
                _logger.LogInformation("Attempting to get product with Id:{ProductId} at {Date}",productId,DateTime.UtcNow);
                var product = await _repository.GetByIdAsync(productId);
                if (product == null) ThrowNotFound($"product with id: {productId} was not found");
                return _mapper.Map<ProductDTO>(product);   
        }

        public async Task<ProductDTO> AddProductAsync(CreateProductDTO productDto)
        {
            _logger.LogInformation("Attempting to add a new product to Database at {Date}", DateTime.Now);
            // Map basic properties
            var product = _mapper.Map<Product>(productDto);
                // Handle categories (reuse if exists)
                product.Categories = new List<Category>();
                foreach (var categoryName in productDto.Categories.Distinct())
                {
                    var existingCategory = await _dbContext.Categories
                        .FirstOrDefaultAsync(c => c.Name == categoryName);

                    product.Categories.Add(existingCategory ?? new Category(categoryName));
                }
                product.Images = await _fileService.UploadFiles(productDto.Images);
                await _repository.CreateAsync(product);
                return _mapper.Map<ProductDTO>(product);  
        }

        public async Task<ProductDTO> UpdateProductAsync(Guid id, UpdateProductDTO product)
        { 
                _logger.LogInformation("Attempting to update product with Id:{ProductId} to Database at {Date}",id, DateTime.Now);
                var existingProduct = await _repository.GetByIdAsync(id);
                if (existingProduct == null) { ThrowNotFound($"product with id: {id} not found"); }

                // Update scalar properties
                existingProduct.Name = product.Name;
                existingProduct.Summary = product.Summary;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Dimensions = product.Dimensions;
                existingProduct.LastUpdatedAt = DateTime.UtcNow;

                // Update categories (clear and re-add references)
                existingProduct.Categories.Clear();
                foreach (var categoryName in product.Categories.Distinct())
                {
                    var existingCategory = await _dbContext.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
                    existingProduct.Categories.Add(existingCategory ?? new Category(categoryName.ToLower()));
                }

                // Update images (remove old, add new)
                List<string> removedPublicIds = new();
                foreach (var imageUrl in product.RemovedFiles)
                {
                    var item = existingProduct.Images.FirstOrDefault(x => x.Url == imageUrl);
                    if (item != null)
                    {
                        existingProduct.Images.Remove(item);
                        //for testing purposes add empty string
                        if (item.PublicId != null)
                            removedPublicIds.Add(item.PublicId);
                    }
                    // call deleteFiles from file service to permanently delete this files
                }
                //enable to delete files beware of onCascade delete,keep for your records
                //if (removedPublicIds.Any())
                //{
                //    await _fileService.DeleteFiles(removedPublicIds);
                //}
                var newUploads = await _fileService.UploadFiles(product.NewFiles);
                foreach (var image in newUploads)
                {
                    existingProduct.Images.Add(image);
                }

                await _dbContext.SaveChangesAsync();
                return _mapper.Map<ProductDTO>(existingProduct);   
        }
        public async Task RemoveProductByIdAsync(Guid productId)
        {
            _logger.LogInformation("Attempting to delete product with Id:{ProductId} from Database at {Date}",productId, DateTime.Now);
            var deleted =await _repository.DeleteAsync(productId);
             if (!deleted) ThrowNotFound($"product with id: {productId} not found"); 
        }
        public async Task<IEnumerable<ProductDTO>> GetProductsByNameAsync(string name)
        {
            _logger.LogInformation("Attempting to get all products by name:{ProductName} from Database at {Date}",name, DateTime.Now);
            var products = await _repository.SearchByNameAsync(name);
            return _mapper.Map<IEnumerable<Product>,IEnumerable<ProductDTO>>(products); 
        }
        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(string categoryName)
        {
            _logger.LogInformation("Attempting to get all products in category:{Category} from Database at {Date}", categoryName, DateTime.Now);
            var category = _dbContext.Categories.FirstOrDefault(c => c.Name.ToLower() == categoryName.Trim().ToLower());
            if (category == null) ThrowBadRequest($"there is no category by name {categoryName}");
            var products= await _repository.SearchByCategoryAsync(categoryName);
            return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);   
        }
    }
}
