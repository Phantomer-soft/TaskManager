using System.ComponentModel.DataAnnotations;

namespace TaskManager.Data.Models.Dtos.TaskDtos;

public class CreateTaskRequestDto
{
    [Required]
    [MinLength(3, ErrorMessage = "Gorev en az 3 karakter olmalidir")]
    public string Description { get; set; } = string.Empty;
}