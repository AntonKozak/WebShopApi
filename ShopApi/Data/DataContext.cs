using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApi.Entities;

namespace ShopApi.Data;

public class DataContext : IdentityDbContext<
UserModel, 
AppRole, int, 
IdentityUserClaim<int>,
AppUserRole,
IdentityUserLogin<int>,
IdentityRoleClaim<int>,
IdentityUserToken<int>
 >
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<Cactus> Cacti { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CactusPhoto> CactusPhotos { get; set; }
    public DbSet<UsersPhoto> UsersPhotos { get; set; }
    public DbSet<UsersLikes> UsersLikes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        modelBuilder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        modelBuilder.Entity<UsersLikes>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        //Anton likes many users
        modelBuilder.Entity<UsersLikes>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //Many users likes Anton
        modelBuilder.Entity<UsersLikes>()
             .HasOne(s => s.TargetUser)
             .WithMany(l => l.LikedByUsers)
             .HasForeignKey(s => s.TargetUserId)
             .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
}

