using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth22.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace auth22.Services;
public class JwtService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public JwtService(IConfiguration config,SymmetricSecurityKey key)
    {
        _config=config;
        var secretKey = _config["JwtService:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
    }

    public string GenerateToken(User user)
    {      
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.UserName ),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Audience = _config["JwtSettings:"],
            Issuer = _config["JwtSettings:Issuer"],
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpInMinutes"])),
            SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}