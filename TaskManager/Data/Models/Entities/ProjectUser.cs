namespace TaskManager.Data.Models.Entities;
public enum Role
{
    Admin,
    Participant,
    Viewer
}
public class ProjectUser
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Role Role { get; set; } = Role.Viewer;

}