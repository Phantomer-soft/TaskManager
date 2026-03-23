using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Models.Dtos;
using TaskManager.Data.Models.Dtos.ProjectDtos;
using TaskManager.Data.Models.Dtos.TaskDtos;
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

    // yeni bir proje olusturur 
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody]CreateProjectDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null) 
            return Unauthorized();
        else
        {
            var user =  await _dbContext.Users.FirstOrDefaultAsync(u=> u.Id == Guid.Parse(userId));
            if(user==null)
                return  Unauthorized();
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

    // projelerin baslik ve aciklama bilgilerini listeler
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
                            Id =  p.ProjectId,
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

    // bir proje icin ayrintili bilgileri getirir 
    [HttpGet("GetProjectInfo")]
    public IActionResult GetProjectInfo([FromBody] Guid id)
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
                })
                .FirstOrDefault();
            if(project == null)
                return NotFound(new {message = "Proje bulunamadi"});
            return Ok(project);
        }
        return Unauthorized();
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProjectDetails(Guid projectId, [FromBody]UpdateProjectRequestDto request)
    {
        var  userId = User.FindFirst("UserId")?.Value;
        if(userId == null)
            return Unauthorized(new {message = "Lutfen tekrar giris yapiniz"});
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Lutfen proje status degerini Ready, InProgress veya Done olarak ayarlayin "});
        var projectUser = await _dbContext.ProjectUsers
            .Include(pu => pu.Project)
            .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                       && pu.ProjectId == projectId
                                       && (pu.Role == Role.Admin || pu.Role == Role.Participant));
        if (projectUser == null)
        {
            return Unauthorized(new {message =  "Proje bulunamadi veya yetkiniz yok"});
        }
        else
        {
            projectUser.Project.Description = request.Description ?? projectUser.Project.Description;
            projectUser.Project.Header = request.Header ?? projectUser.Project.Header;
            projectUser.Project.Status = request.ProjectStatus ?? projectUser.Project.Status;
            await _dbContext.SaveChangesAsync();
            SendProjectInfoDto dto = new()
            {
                ProjectId = projectUser.Project.Id,
                Description = projectUser.Project.Description,
                Header = projectUser.Project.Header,
                Status = projectUser.Project.Status.ToString(),
            };
            return Ok(new {message = "Proje guncellendi",dto});
        }
    }
    //                                   TASKS 
    [HttpPost("{projectId}/Tasks")]
    public async Task<IActionResult> CreateTask(Guid projectId,[FromBody]List<CreateTaskRequestDto> taskList)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            return Unauthorized();
        if(taskList.Count < 1)
            return BadRequest(new {message = "En az 1 gorev eklemelisiniz "});
        else
        {
            // proje id dogru olsa bile kullanici yetkili olmadigi surece ekleme yapamaz
            var projectUser = await _dbContext.ProjectUsers
                .Include(projectUser => projectUser.Project).ThenInclude(project => project.Tasks)
                .FirstOrDefaultAsync(u => u.ProjectId == projectId
                                          && u.UserId == Guid.Parse(userId)
                                          && (u.Role == Role.Admin ||
                                              u.Role == Role.Participant));
            if(projectUser == null)
                return Unauthorized(new {mesage = "Proje bulunamadi veya yetkiniz yok"});
            else
            {
                projectUser.Project.Tasks ??= new List<ProjectTask>(); // ilk gorevse listeyi olusturdum 
                foreach (var task in taskList)
                {
                    ProjectTask projectTask = new()
                    {
                        // task index ornegin 10 tane goerv varsa son eklenmis gorev [9] olacagi icin direkt 10 indexini alarak ekleniyor
                        Id =  Guid.NewGuid(),
                        IsDeleted = false,
                        Description =  task.Description,
                        ProjectId = projectId,
                    };
                    projectUser.Project.Tasks.Add(projectTask);
                    await _dbContext.ProjectTasks.AddAsync(projectTask);
                }
                await _dbContext.SaveChangesAsync();
                SendProjectInfoDto dto = new()
                {
                    ProjectId = projectId,
                    Header = projectUser.Project.Header,
                    Description = projectUser.Project.Description,
                    Status = projectUser.Project.Status.ToString(),
                    Tasks = projectUser.Project.Tasks.Where(t => !t.IsDeleted).ToList()
                };
                
                return Ok(dto);
            }
        }
        
    }
    [HttpPut("{taskId}/Tasks")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody]UpdateTaskRequestDto  request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new {message = "Lutfen status degerini Ready,InProgress veya Done olarak ayarlayin "});
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null) return Unauthorized();

        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

        if (task == null)
            return NotFound(new { message = "Task bulunamadi" });

        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                       && pu.ProjectId == task.ProjectId
                                       && (pu.Role == Role.Admin || pu.Role == Role.Participant));

        if (projectUser == null)
            return Unauthorized(new { message = "Yetkiniz yok" });
        
        task.Description = request.Description;
        task.Status = request.Status;

        await _dbContext.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{taskId}/Tasks")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            return Unauthorized();
        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
        if (task == null)
            return Unauthorized();
        else
        {
            var projectUser = await _dbContext.ProjectUsers
                .Include(pu => pu.Project)
                .ThenInclude(p => p.Tasks)
                .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                           && pu.ProjectId == task.ProjectId
                                           && (pu.Role == Role.Admin || pu.Role == Role.Participant));

            if (projectUser == null)
                return Unauthorized(new { message = "Task bulunamadi veya yetkiniz yok" });

            task.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            SendProjectInfoDto dto = new()
            {
                ProjectId = task.ProjectId,
                Header = projectUser.Project.Header,
                Description = projectUser.Project.Description,
                Status = projectUser.Project.Status.ToString(),
                Tasks = projectUser.Project.Tasks!.Where(t => !t.IsDeleted).ToList()
            };

            return Ok(dto);

        }
    }
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            return Unauthorized();
        else
        {
            // sadece proje sahibi silebilir
            var userProject = await _dbContext.ProjectUsers
                .Include(pu => pu.Project)
                .FirstOrDefaultAsync(
                    pu => pu.UserId ==  Guid.Parse(userId)
                    && pu.ProjectId == projectId
                    && pu.Project.OwnerId == Guid.Parse(userId)
                    && !pu.Project.IsDeleted);
            if (userProject == null)
                return Unauthorized(new { message = "Proje bulunamadi veya yetkiniz yok" });
            else
            {
                // proje silindikten sonra kullanıcı proje tablosunu da temizledim 
                userProject.Project.IsDeleted = true;
                var projectUsers = _dbContext.ProjectUsers.Where(pu => pu.ProjectId == projectId);
                _dbContext.ProjectUsers.RemoveRange(projectUsers);
                await _dbContext.SaveChangesAsync();
                return Ok("Proje basariyla silindi ");
            }
        }
    }
}