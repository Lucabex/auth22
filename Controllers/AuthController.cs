using auth22.Models;
using auth22.Context;
using Microsoft.AspNetCore.Mvc;
using auth22.DTO;
using Microsoft.EntityFrameworkCore;

namespace auth22.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _client;

    public AuthController(AppDbContext context,IHttpClientFactory client)
    {
        _context = context;
        _client = client;
    }

    [HttpPost("reg")]
    public async Task<IActionResult> RegUser(InputDto dto)
    {
        if(string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("UserName and password are mandatory");
        
        }
        try
        {
            if(await _context.User.AnyAsync(u=> u.UserName == dto.UserName))
        {
            return BadRequest("Usernmae not available");
        }
        var user = new User
        {
            UserName = dto.UserName,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            Message ="User registerd",
            UserName= user.UserName,
            Id = user.Id
        });
        }catch(Exception e)
        {
            return StatusCode(503,"Service not available try again later");
        }
        


    }
}