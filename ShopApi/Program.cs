
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Extensions;

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

// Seeding to Database
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();

    await SeedData.LoadUsersData(context);
    await SeedData.LoadCategoriesData(context);
    await SeedData.LoadCactiData(context);
    await SeedData.LoadUsersPhotosData(context);
    await SeedData.LoadCactiPhotosData(context);
}
catch(Exception ex){
    Console.WriteLine("{0}-{1}", ex.Message, ex.InnerException!.Message);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
