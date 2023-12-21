
using System.ComponentModel.DataAnnotations;

namespace ShopApi.DTOs;
public class RegisterDto
{
    [Required]
    public string? UserName { get; set; }
    // public string? KnownAs { get; set; }
    // public string? Gender { get; set; }
    // public DateTime DateOfBirth { get; set; }
    // public string? City { get; set; }
    // public string? Country { get; set; }
    [Required]
 
    public string? Password { get; set; }
}