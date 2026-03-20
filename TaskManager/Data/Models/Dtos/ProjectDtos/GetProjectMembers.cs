namespace TaskManager.Data.Models.Dtos;

public class GetProjectMembers
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
}