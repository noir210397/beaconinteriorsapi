using beaconinteriorsapi.Mappers;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Repositories;
using beaconinteriorsapi.Services;
using beaconinteriorsapi.Validators;
using FluentValidation;
using System.Security.Claims;


namespace beaconinteriorsapi.DependencyInjection
{
    public static class AppDIServicesCollection
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
            services.AddScoped<JwtService>();
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<Seeder>();
            services.AddScoped<ClaimsPrincipal>((s) =>
            {
                var context = s.GetService<IHttpContextAccessor>();
                // if(context == null || context.HttpContext == null || context.HttpContext.User == null)
                // {
                //     return new ClaimsPrincipal();
                // }
                return context!.HttpContext!.User;
            });
            return services;
        }
    }
}
