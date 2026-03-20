using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos;

public class RegisterDto
{
    [MinLength(4,ErrorMessage = "Kullanici adi en az 4 karakter olmalidir")]
    public string UserName { get; set; } =  string.Empty;
    [MinLength(6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string Password { get; set; } =  string.Empty;
    [EmailAddress(ErrorMessage = "Lutfen gecerli bir email adresi giriniz")]
    public string Email { get; set; } =  string.Empty;
    [MinLength(2,ErrorMessage = "Ad en az 2 karakter olmalidir")]
    public string FirstName { get; set; } =  string.Empty;
    [MinLength(2,ErrorMessage = "Soyad en az 2 karakter olmalidir")]
    public string LastName { get; set; } =  string.Empty;
    
}