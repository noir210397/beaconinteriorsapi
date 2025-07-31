using beaconinteriorsapi.Data;


namespace beaconinteriorsapi.Services
{
    public class AppStartupService
    {
       
        private readonly ILogger<AppStartupService> _logger;
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly Seeder _seeder;

        public AppStartupService(ILogger<AppStartupService> logger,BeaconInteriorsDBContext dbContext,Seeder seeder)
        {
            
            _logger = logger;
            _dbContext = dbContext;
            _seeder = seeder;
        }
        public async Task Initialize()
        {
            try
            {
                _dbContext.Database.EnsureDeleted();
                _dbContext.Database.EnsureCreated();
                await _seeder.RunFileUploadTestAsync();
                await _seeder.SeedCategories();
                await _seeder.SeedProductsAsync();
                await _seeder.SeedOrdersAsync();
                await _seeder.SeedRolesAsync();
                await _seeder.SeedUsersAsync();
                _logger.LogInformation("seeding and test completed");

            }
            catch (Exception e)
            {
                _logger.LogError($"unable to complete seeding and file upload test due to error:{e.Message}");
            }
        }
    }
}
