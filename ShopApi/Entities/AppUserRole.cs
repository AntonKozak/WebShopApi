using Microsoft.AspNetCore.Identity;

namespace ShopApi.Entities;

public class AppUserRole: IdentityUserRole<int>
{
    public UserModel User { get; set; }
    public AppRole Role { get; set; }
}
