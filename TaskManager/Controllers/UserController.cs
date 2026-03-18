using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models.Dtos;
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
    public async Task<IActionResult> Register(RegisterDto request)
    {
            var isUsedMail = await _dbContext.Users.AnyAsync(x => x.email == request.email);
            var isUsedUsername = await _dbContext.Users.AnyAsync(x => x.userName == request.userName);
            
            if (isUsedUsername || isUsedMail)// burada daha aciklayici bir mesaj eklenebilir ama bu sefer de guvenlik acisindan goze batan bir durum olur
                return BadRequest(new { message = "Bu email veya kullanici adi kullaniliyor " });
            
            User user = new User()
            {   
                firstName = request.firstName,
                lastName = request.lastName,
                email = request.email,
                userName = request.userName,
                password = request.password  
            
            };
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Ok(user);
          
                    
    }
}