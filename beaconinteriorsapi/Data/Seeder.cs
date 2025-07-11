using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

public  class Seeder
{
   private readonly BeaconInteriorsDBContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ILogger _logger;
    public  string? RootPath;
    private List<ProductDTO>? Products { get; set; }
    private string? ImageFolder { get; set; }
    public Seeder (BeaconInteriorsDBContext context,IMapper mapper,IFileService fileService,ILogger logger)
	{
        _mapper = mapper;
        _context = context;
        _fileService = fileService;
        _logger = logger;
         RootPath = Environment.GetEnvironmentVariable("PATH");
        if (RootPath != null)
        {
            Products = JsonSerializer.Deserialize<List<ProductDTO>>(File.ReadAllText($"{RootPath.Trim()}data.txt"));
            ImageFolder = $"{RootPath.Trim()}images";
        }
        else throw new Exception("PATH is not set in environment variables");
        
    } 
    public async Task SeedCategories()
    {
        if (Products != null) {
            var categories = Products.SelectMany(p=>p.Categories).DistinctBy((c)=>c.Name);
            foreach (var category in categories)
            {
                _context.Categories.Add(new Category() { Name=category.Name});
            }
           await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("no categories found");
        }
        
    }
	public async Task SeedProductsAsync()
	{
        if (Products != null) { 
            var idList=new List<string>();
        foreach (var product in Products) {
            var savedCategories=await _context.Categories.ToListAsync();
            var categoriesList = new List<Category>();
            var imagesList = new List<Image>();
            product.Images.ForEach((image) =>
			{
				imagesList.Add(new Image(image));

            });
            foreach(var category in product.Categories)
            {
                var savedCategory= savedCategories.FirstOrDefault(c=>c.Name.ToLower()==category.Name.ToLower());
                if (savedCategory != null) { 
                
                categoriesList.Add(savedCategory);
                }
                else { categoriesList.Add(new Category(category.Name)); }

            }
                var productToSave = _mapper.Map<Product>(product);

                productToSave.Images=imagesList;
                productToSave.Categories=categoriesList;
                productToSave.CreatedAt= DateTime.Now;
                productToSave.LastUpdatedAt= DateTime.Now;
                productToSave.Id= Guid.NewGuid();
                idList.Add(productToSave.Id.ToString());
                 _context.Products.Add(productToSave);
                //save each product
                await _context.SaveChangesAsync();
		}
            if (RootPath != null)
            {
            var path = $"{RootPath.Trim()}ids.txt";
                using (StreamWriter outputFile = new StreamWriter(path))
                {
                    foreach (string id in idList)
                        outputFile.WriteLine(id);
                }
            }
        }

    }

    public async Task RunTestAsync()
    {
        if (string.IsNullOrWhiteSpace(ImageFolder) || !Directory.Exists(ImageFolder))
        {
            _logger.LogWarning("Image folder not found or not configured.");
            return;
        }

        List<IFormFile> formFiles = new();

        foreach (string filePath in Directory.GetFiles(ImageFolder))
        {
            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            await fs.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var file = new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            formFiles.Add(file);
        }

        _logger.LogInformation("⬆ Uploading images...");

        try
        {
            List<Image> uploaded = await _fileService.UploadFiles(formFiles);
            _logger.LogInformation("Uploaded Images:");
            foreach (var file in uploaded)
            {
                _logger.LogInformation($" URL: {file.Url}, ID: {file.Id}");
            }

            _logger.LogInformation("Deleting uploaded images...");
            var ids = uploaded.Select(u => u.PublicId??"west").ToList();
            bool deleted = await _fileService.DeleteFiles(ids);
             _logger.LogInformation(deleted ? "All deleted." : "Some failed.");
             
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload or delete failed: " + ex.Message);
        }
    }

}

public class SeedFileTest
{
   
}