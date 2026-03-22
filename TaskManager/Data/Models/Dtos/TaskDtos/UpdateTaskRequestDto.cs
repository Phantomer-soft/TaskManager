using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskManager.Data.Models.Entities.TaskStatus;

namespace TaskManager.Data.Models.Dtos.TaskDtos;

public class UpdateTaskRequestDto
{
    [Required]
    [MinLength(3,ErrorMessage = "Aciklama en az  3 karakter olmalidir")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TaskStatus  Status { get; set; } 
    
}