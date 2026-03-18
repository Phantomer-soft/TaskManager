using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;

namespace TaskManager.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
}
