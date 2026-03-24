using TaskStatus = TaskManager.Data.Models.Entities.TaskStatus;

namespace TaskManager.Data.Models.Dtos.TaskDtos;

public class UpdateTaskResponse
{
    public Guid TaskId { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}