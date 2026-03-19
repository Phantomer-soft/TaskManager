using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos;

public class RegisterDto
{
    [MinLength(4,ErrorMessage = "Kullanici adi en az 4 karakter olmalidir")]
    public string userName { get; set; } =  string.Empty;
    [MinLength(6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string password { get; set; } =  string.Empty;
    [EmailAddress(ErrorMessage = "Lutfen gecerli bir email adresi giriniz")]
    public string email { get; set; } =  string.Empty;
    [MinLength(2,ErrorMessage = "Ad en az 2 karakter olmalidir")]
    public string firstName { get; set; } =  string.Empty;
    [MinLength(2,ErrorMessage = "Soyad en az 2 karakter olmalidir")]
    public string lastName { get; set; } =  string.Empty;
    
}