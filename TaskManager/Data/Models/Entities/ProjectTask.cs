namespace TaskManager.Data.Models.Entities;

public enum TaskStatus 
{
    Ready, 
    InProgress,
    Done
        
}
public class ProjectTask
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } =  TaskStatus.Ready;
    public bool IsDeleted { get; set; } =  false;
    public int TaskIndex { get; set; } 
}