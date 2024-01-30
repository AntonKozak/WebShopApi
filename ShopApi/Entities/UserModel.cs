
using Microsoft.AspNetCore.Identity;
using ShopApi.Entities;
namespace ShopApi;

public class UserModel: IdentityUser<int>
{

    public string NickName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AboutMe { get; set; }
    public string Country { get; set; }
    public string City { get; set; }

    public DateOnly DateOfBirth { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public ICollection<UsersPhoto> Photos { get; set; } = new List<UsersPhoto>();
    public ICollection<UsersLikes> LikedByUsers { get; set; }
    public ICollection<UsersLikes> LikedUsers { get; set; }
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }
}