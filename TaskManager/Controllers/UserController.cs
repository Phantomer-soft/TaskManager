using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Auth;
using TaskManager.Data;
using TaskManager.Data.Models.Dtos;
using TaskManager.Data.Models.Entities;
using TaskManager.Models.Dtos;

namespace TaskManager.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")] // direkt endpoint ismiyle ulasabilmek icin 
public class UserController : ControllerBase
{
    private readonly ApiDbContext _dbContext; // dependency inject 
    private readonly JwtTokenHelper _jwtTokenHelper;
    private readonly IConfiguration _config;
    public UserController(ApiDbContext dbContext, JwtTokenHelper jwtTokenHelper, IConfiguration configuration, IConfiguration config)
    {
        _dbContext = dbContext;
        _jwtTokenHelper = jwtTokenHelper;
        _config = config;
    }
    
    [AllowAnonymous]
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
                password = Functions.Hash(request.password),  
            
            };
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            // debug kolay olsun diye simdilik direkt user dondum
            return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var passwordHash = Functions.Hash(request.password);
        var isUser = _dbContext.Users.FirstOrDefault(u => u.userName == request.userName);
        if (isUser != null && Functions.Verify(request.password, isUser.password))
        {
            var token = _jwtTokenHelper.CreateAccessToken(_config,isUser);
            var refreshToken = _jwtTokenHelper.CreateRefreshToken(_config,isUser.id);
            
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            isUser.refreshTokens.Add(refreshToken);
            _dbContext.Update(isUser);
            await _dbContext.SaveChangesAsync();
            // debug kolay olsun diye ikisini birden dondum 
            return Ok(new { token,refreshToken });
        }
        return Unauthorized(new { message = "Kullanici adi veya sifre hatali"});
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordDto request)
    {
        // dogrulanmis olan kullanicinin tokenindan gelen userid degeri
        var userId = User.FindFirst("UserId")?.Value;
        var user = await _dbContext.Users.Include(user => user.refreshTokens)
            .FirstOrDefaultAsync(u => userId != null && u.id == Guid.Parse(userId));

        if (user != null)
        {
            var isPasswordTrue = Functions.Verify(request.oldPassword, user.password);
            if (isPasswordTrue)
            {
                // Not : burada su sekilde bir durum ortaya cikiyor refreshTokeni veritabaninda tuttugum icin gecersiz kilabiliyorum
                // Ama ayni seyi accesstoken icin yapamiyorum onu veritabanina yazmak da karmasikligi artirabilir diye accesstoken suresini kisa tuttum
                
                user.password = Functions.Hash(request.newPassword);
                
                var accessToken = _jwtTokenHelper.CreateAccessToken(_config,user);
                var refreshToken = _jwtTokenHelper.CreateRefreshToken(_config,user.id);

                var userTokens = user.refreshTokens;
                foreach (var token in userTokens)
                {
                    token.isUsed = true;
                    token.isRevoked = true;
                }
                user.refreshTokens.Add(refreshToken);
                _dbContext.Update(user);
                await _dbContext.AddAsync(refreshToken);
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    message = "Sifreniz basariyla guncellendi",
                    accessToken,
                    refreshToken = refreshToken.token,
                });
            }
            return BadRequest(new { message = "Mevcut sifrenizi hatali girdiniz" });
        }
        else
            return NotFound(new {message = "Kullanici bilgileri alinamadi"});
    }
}
