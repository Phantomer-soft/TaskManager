using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos;

public class UpdatePasswordDto
{
    [MinLength(6,ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string OldPassword { get; set; }  = string.Empty;
    [MinLength(6, ErrorMessage = "Yeni sifre  en az 6 karakter olmalidir")]
    public string NewPassword { get; set; }   = string.Empty;
}