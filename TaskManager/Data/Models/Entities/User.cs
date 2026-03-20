using Microsoft.EntityFrameworkCore;
using TaskManager.Auth;

namespace TaskManager.Data.Models.Entities;
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(UserName), IsUnique = true)] 
public class User
{
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } =  string.Empty;
    public string LastName { get; set; } =  string.Empty;
    public string Email { get; set; } =  string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<RefreshToken>RefreshTokens { get; set; } = new List<RefreshToken>();

}