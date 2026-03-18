namespace TaskManager.Models.Entities;

public class User
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string userName { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;

}