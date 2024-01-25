
namespace ShopApi.Helpers;

public class UserParams : PaginationParams
{
    public string CurrentUsername { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string OrderBy { get; set; } = "lastActive";
}

