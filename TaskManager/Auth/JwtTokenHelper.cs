using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Data.Models.Entities;

namespace TaskManager.Auth;

public class JwtTokenHelper
{
    public Token CreateAccessToken(IConfiguration config,User user)
    {
        Token token = new();

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt_Token:SecretKey"]));
        
        SigningCredentials credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha512);
        token.expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt16(config["Jwt_Token:ExpirationMins"]));  // app settingste ayarlardigim exp zamani 

        JwtSecurityToken securityToken = new(
            issuer: config["Jwt_Token:Issuer"],
            audience: config["Jwt_Token:Audience"],
            expires: token.expiration,
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials,
            claims:
            [
                new Claim("userId",user.id.ToString()),
                new Claim("firstName",user.firstName),
                new Claim("lastName",user.lastName),
                new Claim("email",user.email)
            ]
        );
            JwtSecurityTokenHandler tokenHandler = new();
            token.accessToken = tokenHandler.WriteToken(securityToken);

            return token;

    }

    public RefreshToken CreateRefreshToken(IConfiguration config,Guid userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        RefreshToken refreshToken = new RefreshToken
        {
            UserId = userId,
            expiresAt = DateTime.UtcNow.AddHours(Convert.ToInt16(config["Jwt_Token:RefreshTokenExpirationHours"])),
            isRevoked = false,
            isUsed = false,
            token = Convert.ToBase64String(randomBytes),
        };
       return  refreshToken;
    }

}