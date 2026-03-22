namespace TaskManager.Data.Models.Dtos;

public class GetProjectsDto
{
    public Guid Id { get; set; }
    public string Header { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string Role { get; set; }  = string.Empty;
    public string Status { get; set; }  = string.Empty;
}