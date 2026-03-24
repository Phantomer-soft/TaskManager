using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Data.Models.Dtos.ProjectDtos;

public class AddUserRequestDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Role UserRole { get; set; }
}