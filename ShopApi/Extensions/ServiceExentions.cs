
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Interfaces;
using ShopApi.Services;

namespace ShopApi.Extensions;

public static class ServiceExentions
{
    public static IServiceCollection AddServicesExtentions(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen();
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}
