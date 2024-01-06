
using System.ComponentModel.DataAnnotations;

namespace ShopApi.DTOs;
public class RegisterDto
{
    [Required]
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Country { get; set; }
    public string City { get; set; }

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; }
}