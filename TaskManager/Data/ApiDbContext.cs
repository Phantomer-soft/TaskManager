using Microsoft.EntityFrameworkCore;
using TaskManager.Auth;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
}
