using Microsoft.AspNetCore.Identity;

namespace ShopApi.Entities;

public class AppRole: IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}
