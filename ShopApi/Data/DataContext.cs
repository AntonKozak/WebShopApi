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
    public DbSet<UsersLikes> UsersLikes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       base.OnModelCreating(modelBuilder);
       modelBuilder.Entity<UsersLikes>()
           .HasKey(k => new {k.SourceUserId, k.TargerUserId});

        //Anton likes many users
       modelBuilder.Entity<UsersLikes>() 
           .HasOne(s => s.SourceUser)
           .WithMany(l => l.LikedUsers)
           .HasForeignKey(s => s.SourceUserId)
           .OnDelete(DeleteBehavior.Cascade);
        
        //Many users likes Anton
         modelBuilder.Entity<UsersLikes>() 
              .HasOne(s => s.TargerUser)
              .WithMany(l => l.LikedByUsers)
              .HasForeignKey(s => s.TargerUserId)
              .OnDelete(DeleteBehavior.NoAction);
       }
    public DataContext(DbContextOptions options) : base(options) { }
}

