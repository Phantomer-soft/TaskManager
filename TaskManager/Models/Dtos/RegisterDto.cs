using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.Dtos;

public class RegisterDto
{
    public string userName { get; set; } =  string.Empty;
    [MinLength(6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string password { get; set; } =  string.Empty;
    [EmailAddress(ErrorMessage = "Lutfen gecerli bir email adresi giriniz")]
    public string email { get; set; } =  string.Empty;
    public string firstName { get; set; } =  string.Empty;
    public string lastName { get; set; } =  string.Empty;
    
}