using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos.ProjectDtos;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Proje basligi zorunludur")]
    [MinLength(5,  ErrorMessage = "Proje basligi en az 5 karakter olmalidir")]
    public string Header { get; set; } = string.Empty;
    [MinLength(5, ErrorMessage = "Proje aciklamasi en az 5 karakter olmalidir")]
    public string Description { get; set; } = string.Empty;
    
}