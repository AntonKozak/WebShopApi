using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShopApi.Entities;

namespace ShopApi.Data
{
    public class SeedData
    {
        public static async Task LoadUsersData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.Users.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<UserModel>>(json, options);

            if (users is not null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    user.UserName = user.UserName?.ToLower().Trim();

                    context.Users.Add(user);
                }

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No users found");
            }
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