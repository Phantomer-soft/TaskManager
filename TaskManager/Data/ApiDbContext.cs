using Microsoft.EntityFrameworkCore;
using TaskManager.Auth;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project>Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        // classta enum olarak verdigim degerleri string olarak tutsun istedigim icin ekledim 
        
        modelBuilder.Entity<Project>()
            .Property(p => p.Status)
            .HasConversion<string>();
        modelBuilder.Entity<ProjectUser>()
            .Property(p => p.Role)
            .HasConversion<string>();
    }
}
