using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos;

public class UpdatePasswordDto
{
    [MinLength(6,ErrorMessage = "Sifre en az 6 karakter olmalidir")]
    public string oldPassword { get; set; }  = string.Empty;
    [MinLength(6, ErrorMessage = "Yeni sifre  en az 6 karakter olmalidir")]
    public string newPassword { get; set; }   = string.Empty;
}