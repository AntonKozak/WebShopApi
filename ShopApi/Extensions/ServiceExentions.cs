
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Data.Repositories;
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
        services.AddScoped<IUserRepository, UserRepository>(); 
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        
        // services.AddControllers()
        //     .AddJsonOptions(options =>
        //     {
        //         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        //     });
        return services;
    }
}
