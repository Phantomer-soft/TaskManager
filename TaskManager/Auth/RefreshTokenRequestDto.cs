using System.ComponentModel.DataAnnotations;

namespace TaskManager.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; }
}