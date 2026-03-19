using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos;

public class LoginDto
{
    [MinLength(4,ErrorMessage = "Kullanici adi en az 4 karakter olmalidir")]
    public string userName { get; set; } =  string.Empty;
    [MinLength(6,ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string password { get; set; }  =  string.Empty;
}