
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Entities;

namespace ShopApi.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        
        services.AddIdentityCore<UserModel>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
        })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]??"super secret key")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratorRole", policy => policy.RequireRole("Moderator", "Admin"));
            opt.AddPolicy("UserRole", policy => policy.RequireRole("User", "Moderator", "Admin"));
        });

        return services;
    }
}
