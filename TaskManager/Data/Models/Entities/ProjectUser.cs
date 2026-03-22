namespace TaskManager.Data.Models.Entities;
public enum Role
{
    Admin,
    Participant,
    Viewer
}
public class ProjectUser
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;  
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = Role.Viewer;

}