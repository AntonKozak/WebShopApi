using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using ShopApi.Entities;

namespace ShopApi.Data
{
    public class SeedData
    {
        public static async Task LoadUsersData(UserManager<UserModel> userManager, RoleManager<AppRole> roleManager)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
                new AppRole{Name = "User"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);   
            }

            if (userManager.Users.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<UserModel>>(json, options);

            if (users is not null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    user.UserName = user.UserName?.ToLower().Trim();
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }else
            {
                Console.WriteLine("No users found");
            }

            //addink admin user and moderaotor user to run some tests on them 
            var admin = new UserModel
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator", "User" });

            var moderator = new UserModel
            {
                UserName = "moderator"
            };

            await userManager.CreateAsync(moderator, "Pa$$w0rd");
            await userManager.AddToRolesAsync(moderator, new[] { "Moderator", "User" });

        }
        public static async Task LoadCactiData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.Cacti.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/CactusSeedData.json");

            var cacti = JsonSerializer.Deserialize<List<Cactus>>(json, options);

            if (cacti is not null && cacti.Count > 0)
            {
                await context.Cacti.AddRangeAsync(cacti);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No cacti found");
            }
        }
        public static async Task LoadCategoriesData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.Categories.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/CategorySeedData.json");

            var categories = JsonSerializer.Deserialize<List<Category>>(json, options);

            if (categories is not null && categories.Count > 0)
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No categories found");
            }
        }

        public static async Task LoadUsersPhotosData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.UsersPhotos.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/PhotoSeedData.json");

            var usersPhotos = JsonSerializer.Deserialize<List<UsersPhoto>>(json, options);

            if (usersPhotos is not null && usersPhotos.Count > 0)
            {
                await context.UsersPhotos.AddRangeAsync(usersPhotos);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No users photos found");
            }
        }

        public static async Task LoadCactiPhotosData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.CactusPhotos.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/CactusPhotoSeedData.json");

            var cactiPhotos = JsonSerializer.Deserialize<List<CactusPhoto>>(json, options);

            if (cactiPhotos is not null && cactiPhotos.Count > 0)
            {
                await context.CactusPhotos.AddRangeAsync(cactiPhotos);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No cactuc photos found");
            }
        }

    }
}