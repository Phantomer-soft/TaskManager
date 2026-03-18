using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models.Entities;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]/[action]")] // direkt endpoint ismiyle ulasabilmek icin 
public class UserController : ControllerBase
{
    private readonly ApiDbContext _dbContext; // dependency inject 

    public UserController(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        User user = new User()
        {
            userName = email,
            password = password
        };
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return Ok(user);
    }
}