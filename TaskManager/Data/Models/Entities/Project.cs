namespace TaskManager.Data.Models.Entities;
public enum Status
{
    Ready,
    InProgress,
    Done
}
public class Project
{
    public Project()
    {
        
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; } 
    public string Header { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } =  false; 
    public Status Status { get; set; } =  Status.Ready;
    
    public ICollection<ProjectTask>? Tasks { get; set; } =  new List<ProjectTask>();
    
}