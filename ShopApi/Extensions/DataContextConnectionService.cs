using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopApi.Data;

namespace ShopApi.Extensions;

public static class DataContextConnectionService
{
public static IServiceCollection AddDataContextConnectionService(this IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
    });
    return services;
}

}
