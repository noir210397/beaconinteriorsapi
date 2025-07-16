using beaconinteriorsapi.Mappers;
using beaconinteriorsapi.Repositories;
using beaconinteriorsapi.Services;
using beaconinteriorsapi.Validators;
using FluentValidation;

namespace beaconinteriorsapi.DependencyInjection
{
    public static class AppDIServicesColllection
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductProfile));
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<IFileService, FileService>();
            services.AddValidatorsFromAssemblyContaining(typeof(CreateProductValidator));
            services.AddScoped<AppStartupService>();
            services.AddScoped<CheckoutService>();
            services.AddScoped<OrderService>();
            return services;
        }
    }
}
