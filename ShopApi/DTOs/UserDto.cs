using System.ComponentModel.DataAnnotations;

namespace ShopApi.DTOs;
public class UserDto
{
    [Required]

    public string? UserName { get; set; }
    // public string? Token { get; set; }
    // public string? PhotoUrl { get; set; }
    // public string? KnownAs { get; set; }
    // public string? Gender { get; set; }
}