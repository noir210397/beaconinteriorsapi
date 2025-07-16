using AutoMapper;
using beaconinteriorsapi.Data;

namespace beaconinteriorsapi.Services
{
    public class AppStartupService
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<AppStartupService> _logger;
        private readonly BeaconInteriorsDBContext _dbContext;
        public AppStartupService(IMapper mapper,IFileService fileService,ILogger<AppStartupService> logger,BeaconInteriorsDBContext dbContext)
        {
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task Initialize()
        {
            try
            {
                var seeder = new Seeder(_dbContext, _mapper, _fileService,_logger);
                _dbContext.Database.EnsureDeleted();
                _dbContext.Database.EnsureCreated();
                await seeder.RunTestAsync();
                await seeder.SeedCategories();
                await seeder.SeedProductsAsync();
                await seeder.SeedOrdersAsync();
                _logger.LogInformation("seeding and test completed");

            }
            catch (Exception e)
            {
                _logger.LogError($"unable to complete seeding and file upload test due to error:{e.Message}");
            }
        }
    }
}
