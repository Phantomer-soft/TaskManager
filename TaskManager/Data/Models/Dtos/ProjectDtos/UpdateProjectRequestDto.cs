using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Data.Models.Dtos.ProjectDtos;

public class UpdateProjectRequestDto
{
    public string? Header { get; set; }
    public string? Description { get; set; }
    [Required]
    public Status? ProjectStatus { get; set; }
}