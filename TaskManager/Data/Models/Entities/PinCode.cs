namespace TaskManager.Data.Models.Entities;

public class PinCode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public Role UserRole { get; set; }
    public bool IsUsed { get; set; } =  false;
    public int Code { get; set; } 
}