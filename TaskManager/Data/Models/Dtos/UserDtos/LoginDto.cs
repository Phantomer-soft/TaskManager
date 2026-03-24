using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos;

public class LoginDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } =  string.Empty;
    [Required]
    [MinLength(6,ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string Password { get; set; }  =  string.Empty;
}