using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Models.Dtos;
using TaskManager.Data.Models.Dtos.ProjectDtos;
using TaskManager.Data.Models.Dtos.TaskDtos;
using TaskManager.Data.Models.Entities;
using TaskManager.GlobalExceptionHandling;

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
    // userid yi her seferinde tekrar almak yerine tek metodla alsam daha guzel olabilirmis 
    //                                PROJECTS
    
    // yeni bir proje olusturur 
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody]CreateProjectDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            throw new UserLoginException("Kullanici bilgileri alinamadi");
        else
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
            if (user == null)
               throw new UserLoginException("Lutfen tekrar giris yapiniz");
            var projectId = Guid.NewGuid();
            Project project = new()
            {
                Id = projectId,
                Header = dto.Header,
                Description = dto.Description,
                OwnerId = Guid.Parse(userId),
                IsDeleted = false,
                Status = Status.Ready
            };
            ProjectUser pUser = new()
            {
                UserId = Guid.Parse(userId),
                Project = project,
                ProjectId = projectId,
                User = user,
                Role = Role.Admin
            };
            await _dbContext.Projects.AddAsync(project);
            await _dbContext.ProjectUsers.AddAsync(pUser);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Proje basariyla olusturuldu " });
        }

    }

    // projelerin baslik ve aciklama bilgilerini listeler
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId != null)
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
                    return Ok(new {message = "Kayitli proje bulunamadi"});
                }
                return Ok(projects);
            
        }
        throw new UserLoginException("Lutfen tekrar giris yapiniz");
    }

    // bir proje icin ayrintili bilgileri getirir 
    [HttpGet("ProjectInfo/{projectId}")]
    public async Task<IActionResult> GetProjectInfo(Guid projectId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId != null)
        {
            var project = await _dbContext.ProjectUsers
                .Where(pu => pu.UserId == Guid.Parse(userId) 
                             && pu.ProjectId == projectId)
                .Include(pu => pu.Project).Select(pu => new SendProjectInfoDto
                {
                    ProjectId = pu.Project.Id,
                    Header = pu.Project.Header.ToString(),
                    Description = pu.Project.Description,
                    Status = pu.Project.Status.ToString(),
                    Tasks = pu.Project.Tasks
                })
                .FirstOrDefaultAsync();
            if(project == null)
                return NotFound(new {message = "Proje bulunamadi"});
            return Ok(project);
        }
        throw new UserLoginException("Lutfen tekrar giris yapiniz");
    }

    // secili project id ye sahip quizi duzenler
    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProjectDetails(Guid projectId, [FromBody]UpdateProjectRequestDto request)
    {
        var  userId = User.FindFirst("UserId")?.Value;
        if(userId == null)
            throw new UserLoginException("Lutfen tekrar giris yapiniz");
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Lutfen proje status degerini Ready, InProgress veya Done olarak ayarlayin "});
        var projectUser = await _dbContext.ProjectUsers
            .Include(pu => pu.Project)
            .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                       && pu.ProjectId == projectId
                                       && (pu.Role == Role.Admin || pu.Role == Role.Participant));
        if (projectUser == null)
        {
            throw new ProjectNotFoundException("Proje bulunamadi veya yetkiniz yok ");
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
    // secili project id icin yeni bir task olusturur 
    [HttpPost("{projectId}/Tasks")]
    public async Task<IActionResult> CreateTask(Guid projectId,[FromBody]List<CreateTaskRequestDto> taskList)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            throw new UserLoginException("Lutfen tekrar giris yapiniz");
        if(taskList.Count < 1)
            return BadRequest(new {message = "En az 1 gorev eklemelisiniz "});
        if (!ModelState.IsValid)
            return BadRequest();
        if(projectId == Guid.Empty)
            return BadRequest(new {message = "Lutfen gecerli bir proje id girin"});
            
            // proje id dogru olsa bile kullanici yetkili olmadigi surece ekleme yapamaz
            var projectUser = await _dbContext.ProjectUsers
                .Include(projectUser => projectUser.Project).ThenInclude(project => project.Tasks)
                .FirstOrDefaultAsync(u => u.ProjectId == projectId
                                          && u.UserId == Guid.Parse(userId)
                                          && (u.Role == Role.Admin ||
                                              u.Role == Role.Participant));
            if (projectUser == null)
                throw new ProjectNotFoundException("Proje bulunamadi veya yetkiniz yok ");
            else
            {
                projectUser.Project.Tasks ??= new List<ProjectTask>(); // ilk gorevse listeyi olusturdum 
                foreach (var task in taskList)
                {
                    ProjectTask projectTask = new()
                    {
                        // task index ornegin 10 tane goerv varsa son eklenmis gorev [9] olacagi icin direkt 10 indexini alarak ekleniyor
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        Description = task.Description,
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
    [HttpPut("{taskId}/Tasks")]
    // verilen task id ye sahip task i gunceller
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody]UpdateTaskRequestDto  request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new {message = "Lutfen status degerini Ready,InProgress veya Done olarak ayarlayin "});
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null) 
            throw new UserLoginException("Lutfen tekrar giris yapiniz");

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
        UpdateTaskResponse taskInfo = new()
        {
            CreatedAt =  task.CreatedAt,
            Description =  task.Description,
            Status = task.Status,
            TaskId = task.Id
        };
        return Ok(new { message = "Gorev guncellendi",taskInfo});
    }

    [HttpPost("adduser")]
    public async Task<IActionResult> AddUserToProject([FromBody]AddUserRequestDto request)
    {
        var userId =  User.FindFirst("UserId")?.Value;
        if (userId == null)
            throw new UserLoginException("Lutfen tekrar giris yapiniz");
        var projectUser = await _dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
            && pu.ProjectId == request.ProjectId
            && pu.Role == Role.Admin);
        if (projectUser == null)
            return Unauthorized(new { message = "Yetkiniz yok" });
        else
        {
            var project = _dbContext.Projects
                .FirstOrDefault(p => p.Id == request.ProjectId  
                                     && !p.IsDeleted);
            if (project == null)
                return  NotFound(new { message = "Proje bulunamadi" });
            Random rnd = new();
            PinCode pinCode = new ()
            {
                Code = rnd.Next(100000,999999),
                ProjectId =  request.ProjectId,
                UserRole = request.UserRole,
                Project = project
            };
            
            await _dbContext.PinCodes.AddAsync(pinCode);
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                message = "Kayıt basarili, katılmasını istediginiz kullanici pin kodunu kullanarak katilabilir",
                pincode = pinCode.Code
            });
        }
    }

    [HttpGet("join/{pinCode}")]
    public async Task<IActionResult> JoinProject(int pinCode)
    {
        var  userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            throw new UserLoginException("Lutfen tekrar giris yapiniz");
        else
        {
            if(pinCode is > 999999 or < 100000)
                return BadRequest(new { message = "Lutfen gecerli bir pin kodu girin" });
            var pinProject = await _dbContext.PinCodes
                .FirstOrDefaultAsync(pp => pp.Code == pinCode 
                                           && !pp.IsUsed);
            var isJoined  = await _dbContext.ProjectUsers
                .AnyAsync(pu => pu.UserId == Guid.Parse(userId) 
                                && pinProject != null 
                                && pu.ProjectId ==pinProject.ProjectId);
            if (isJoined)
                return Unauthorized(new { message = "Bu projeye zaten katildiniz" });
            if (pinProject == null)
                return NotFound(new {message= "Gecersiz pin kodu "});

            var user = await _dbContext.Users.FindAsync(Guid.Parse(userId));
            if(user == null)
                return NotFound(new {message = "Kullanici bulunamadi"});
            ProjectUser projectUser = new()
            {
                UserId = Guid.Parse(userId),
                ProjectId = pinProject.ProjectId,
                Role = pinProject.UserRole,
                User =  user
            };
            pinProject.IsUsed = true;
            await _dbContext.ProjectUsers.AddAsync(projectUser);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Projeye basariyla katildiniz" });
        }
    }

    // verilen task id ye sahip taski silindi isaretler 
    [HttpDelete("{taskId}/Tasks")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            return Unauthorized();
        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
        if (task == null)
            return NotFound(new { message = "Task bulunamadi" });
        else
        {
            var projectUser = await _dbContext.ProjectUsers
                .Include(pu => pu.Project)
                .ThenInclude(p => p.Tasks)
                .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                           && pu.ProjectId == task.ProjectId
                                           && (pu.Role == Role.Admin || pu.Role == Role.Participant));

            if (projectUser == null)
                throw new ProjectNotFoundException("Proje bulunamadi veya yetkiniz yok");

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

            return Ok(new {message = "Silme islemi basarili", dto});

        }
    }
    
    // verilen project id ye sahip gorevi silindi isaretler 
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            throw new UserLoginException("Lutfen tekrar giris yapniz");
        else
        {
            // sadece proje sahibi silebilir
            var userProject = await _dbContext.ProjectUsers
                .Include(pu => pu.Project)
                .FirstOrDefaultAsync(pu => pu.UserId == Guid.Parse(userId)
                                           && pu.ProjectId == projectId
                                           && pu.Project.OwnerId == Guid.Parse(userId)
                                           && !pu.Project.IsDeleted);
            if (userProject == null)
                throw new ProjectNotFoundException("Proje bulunamadi veya yetkiniz yok");
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