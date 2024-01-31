
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopApi;
using ShopApi.Data;
using ShopApi.Entities;
using ShopApi.Error.Middleware;
using ShopApi.Extensions;
using ShopApi.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
});

builder.Services.AddControllers();

builder.Services.AddServicesExtentions(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>(); // ExceptionMiddleware is a custom middleware that we created to handle exceptions(errors)

// Seeding to Database

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<UserModel>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    await context.Database.MigrateAsync();

    await SeedData.LoadUsersData(userManager, roleManager);
    await SeedData.LoadCategoriesData(context);
    await SeedData.LoadCactiData(context);
    await SeedData.LoadUsersPhotosData(context);
    await SeedData.LoadCactiPhotosData(context);
}
catch(Exception ex){
    Console.WriteLine("{0}-{1}", ex.Message, ex.InnerException);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
.AllowAnyMethod()
.AllowCredentials() //for SignalR
.AllowAnyHeader()
.WithOrigins("https://localhost:4200")
);

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresemceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();
