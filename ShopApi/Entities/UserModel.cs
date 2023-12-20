using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace ShopApi;

public class UserModel
{
    public int Id { get; set; }
    public string? Username { get; set; }
    // public string? Password { get; set; }
    // public string? Email { get; set; }
    // public string? FirstName { get; set; }
    // public string? LastName { get; set; }
}