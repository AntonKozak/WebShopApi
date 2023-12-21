using System.ComponentModel.DataAnnotations;
namespace ShopApi;

public class UserModel
{
    public int Id { get; set; }
    [Required]

        public string? UserName { get; set; }
    // public string? Email { get; set; }
    // public string? FirstName { get; set; }
    // public string? LastName { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
}