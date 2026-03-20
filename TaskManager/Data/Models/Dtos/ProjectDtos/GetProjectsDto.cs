namespace TaskManager.Data.Models.Dtos;

public class GetProjectsDto
{
    public Guid Id { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
}