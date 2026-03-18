using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models.Entities;
[Index(nameof(email), IsUnique = true)]
[Index(nameof(userName), IsUnique = true)] 
public class User
{
    
    public Guid id { get; set; } = Guid.NewGuid();
    public string firstName { get; set; } =  string.Empty;
    public string lastName { get; set; } =  string.Empty;
    public string email { get; set; } =  string.Empty;
    public string userName { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;

}