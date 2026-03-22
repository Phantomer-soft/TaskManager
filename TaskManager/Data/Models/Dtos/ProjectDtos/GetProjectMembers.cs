namespace TaskManager.Data.Models.Dtos;

public class GetProjectMembers
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } =  string.Empty;
    
}