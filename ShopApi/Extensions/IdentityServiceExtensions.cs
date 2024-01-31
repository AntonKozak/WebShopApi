
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
                //configuring the token validation parameters for the JwtBearer authentication
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"] ?? "super secret key")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // this is to enable SignalR to use JWT token for authentication; building query parameter specifically for requests targeting the "/hubs" path.
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        //adding authorization policies to the services container to be used in the controllers and hubs 
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratorRole", policy => policy.RequireRole("Moderator", "Admin"));
            opt.AddPolicy("UserRole", policy => policy.RequireRole("User", "Moderator", "Admin"));
        });

        return services;
    }
}
