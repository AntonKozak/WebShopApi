using System.Runtime;
using Microsoft.EntityFrameworkCore;
using ShopApi.Entities;

namespace ShopApi.Data;

public class DataContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<Cactus> Cacti { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CactusPhoto> CactusPhotos { get; set; }
    public DbSet<UsersPhoto> UsersPhotos { get; set; }
    public DataContext(DbContextOptions options) : base(options) { }
}

