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
                    using var hmac = new HMACSHA512();
                    
                    user.UserName = user.UserName?.ToLower().Trim();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt = hmac.Key;
                    

                    context.Users.Add(user);
                }

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No users found");
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

        public static async Task LoadPhotosData(DataContext context)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (context.Photos.Any()) return;

            var json = await File.ReadAllTextAsync("Data/json/PhotoSeedData.json");

            var photos = JsonSerializer.Deserialize<List<Photo>>(json, options);

            if (photos is not null && photos.Count > 0)
            {
                await context.Photos.AddRangeAsync(photos);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No photos found");
            }
            
    }

}
}