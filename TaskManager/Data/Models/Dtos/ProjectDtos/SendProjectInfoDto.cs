using TaskManager.Data.Models.Entities;

namespace TaskManager.Data.Models.Dtos.ProjectDtos;

public class SendProjectInfoDto
{
    public Guid ProjectId{ get; set; }
    public string Header { get; set; } =  string.Empty;
    public string Description { get; set; }  =  string.Empty;
    public string Status { get; set; }  =  string.Empty;
    public ICollection<ProjectTask>? Tasks { get; set; } =  new List<ProjectTask>();
}