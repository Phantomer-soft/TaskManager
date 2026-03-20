using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Models.Dtos;
using TaskManager.Data.Models.Dtos.ProjectDtos;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Controllers;
[Authorize]
[Route("api/[controller]")]
public class ProjectController : Controller
{
    private readonly ApiDbContext _dbContext;

    public ProjectController(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateProject(CreateProjectDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null) 
            return Unauthorized();
        else
        {
            var user =  await _dbContext.Users.FirstOrDefaultAsync(u=> u.Id == Guid.Parse(userId));
            var projectId = Guid.NewGuid();
            Project project = new()
            {
                Id =projectId,
                Header = dto.Header,
                Description = dto.Description,
                OwnerId = Guid.Parse(userId),
                IsDeleted =  false,
                Status = Status.Ready
            };
            ProjectUser pUser = new()
            {
                UserId = Guid.Parse(userId),
                Project =  project,
                ProjectId = projectId,
                User =  user,
                Role = Role.Admin
            };
            await _dbContext.Projects.AddAsync(project);
            await _dbContext.ProjectUsers.AddAsync(pUser);
            await _dbContext.SaveChangesAsync();

            return Ok( new { message = "Proje basariyla olusturuldu "});
        }
        
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            try
            {
                // kullanici giris yaptiktan sonra sahip oldugu veya katilmci olarak eklenmis oldugu her projeyi goruntuleyebilir
                var projects = await _dbContext.ProjectUsers.Where(
                        p => p.UserId == Guid.Parse(userId))
                    .Select(p=> new GetProjectsDto
                        {
                            Id =  p.Id,
                            Description = p.Project.Description,
                            Header = p.Project.Header,
                            Role = p.Role.ToString(),
                            Status = p.Project.Status.ToString()
                        }
                    )
                    .ToListAsync();
                if (projects.Count == 0)
                {
                    return Ok("Kayitli proje bulunamadi");
                }
                return Ok(projects);
            }
            catch
            {
                return NotFound(new {message = "Proje bulunamadi" });
            }
        }
        return Unauthorized();
    }

    [HttpGet("{id}")]
    public IActionResult GetProjectInfo(Guid id)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            var project = _dbContext.ProjectUsers
                .Where(pu => pu.UserId == Guid.Parse(userId) 
                             && pu.ProjectId == id)
                .Include(pu => pu.Project).Select(pu => new SendProjectInfoDto
                {
                    ProjectId = pu.Project.Id,
                    Header = pu.Project.Header.ToString(),
                    Description = pu.Project.Description,
                    Status = pu.Project.Status.ToString(),
                    Tasks = pu.Project.Tasks
                });
            return Ok(project);
        }
        return Unauthorized();
    }
}