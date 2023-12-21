
using System.ComponentModel.DataAnnotations;

namespace ShopApi.DTOs;
public class RegisterDto
{
    [Required] public string? Username { get; set; }
    public string? KnownAs { get; set; }
    public string? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Password { get; set; }
}