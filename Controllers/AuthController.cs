using auth22.Models;
using auth22.Context;
using Microsoft.AspNetCore.Mvc;
using auth22.DTO;
using Microsoft.EntityFrameworkCore;
using auth22.Services;
using auth22.Records;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace auth22.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _client;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext context,IHttpClientFactory client,JwtService jwt)
    {
        _context = context;
        _client = client;
        _jwt = jwt;
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
    [HttpPost("log")]
    public async Task<IActionResult> LogUser(InputDto dto)
    {
        if(string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("UserName and password are mandatory");
        
        }
        try
        {
            var user = await _context.User.FirstOrDefaultAsync(u=> (u.UserName ??  "Unknwon").ToLower() == dto.UserName.ToLower() );
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.UserName, user.HashedPassword))
            {
                return BadRequest("Username or password not available");
            }
            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                message= "User Logged in",
                Token = token,
                UserName = user.UserName,
                userId = user.Id,
                Solved = user.SolvedPuzzle

            });
            
        }catch(Exception e)
        {
            return StatusCode(503,"Service not available");
        }
    }
    [HttpPost("solve")]
    public async Task<IActionResult> AddSolve(AddSolves sol)
    {
        if(sol == null)
        {
            return BadRequest("You must be logged in");
        }

        var userFile = await _context.User.FirstOrDefaultAsync(u=>u.Id  == sol.Id);
        
        if(userFile == null)
        {
            return NotFound("User not found!");
        }
        userFile.SolvedPuzzle+=1;
        
        await _context.SaveChangesAsync();
     
        return Ok(new
        {
            Message="Puzzle added",
            NewCount = userFile.SolvedPuzzle,

        });
    }
    [HttpPost("AddPuzzle")]
    [Authorize]
    public async Task<IActionResult> AddPuzzle()
    {
        var userClaimid = User.FindFirstValue(ClaimTypes.NameIdentifier);
          if (string.IsNullOrEmpty(userClaimid) || !int.TryParse(userClaimid, out int userId))
    {
        return Unauthorized("Invalid token or user ID format");
    }
        var user = await _context.User.FirstOrDefaultAsync(u=>u.Id==userId);
        if(user == null)
        {
            return NotFound("user not found");
        }
        user.Attempt+=1;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            message="attempt added",
            count = user.Attempt
        });
    
    }

    [HttpGet("count")]
    [Authorize]
        public async Task<IActionResult> GetCount()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); 
     if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    {
        return Unauthorized("Invalid token or user ID format");
    }
    
    var user = await _context.User.FirstOrDefaultAsync(u =>u.Id == userId);
    
    if (user == null)
    {
        return NotFound("User not found");
    }
        var userCount = user.SolvedPuzzle;
        Console.WriteLine("this is usercount!!!!!!!!",userCount); 
        return Ok(new
        {
            message="count returned",
            Count=user.SolvedPuzzle
        });
    }
    [HttpGet("puz")]
  
    public async Task<IActionResult> GetPuzzle()
    {
        var client = _client.CreateClient();
        var url= "https://lichess.org/api/puzzle/daily";
        var response = await client.GetFromJsonAsync<DailyPuzzle>(url);

        if(response?.Puzzle?.Fen==null || response.Puzzle.Solution==null || response.Puzzle.Themes==null)
        {
            return StatusCode(503,"Server error");
        }
        
        return Ok(response);
       
    }
    [HttpGet("puz2")]
    public async Task<IActionResult> GetPuzzle2()
    {
        var client = _client.CreateClient();
        var url = "https://chess-puzzles-api.vercel.app/puzzles?themes=mateIn4&min_rating=800&max_rating=2000&limit=20";       
        var response = await client.GetFromJsonAsync<List<Puzzle2>>(url);

    if (response == null || response.Count == 0 || response.Count <20)
    {
        return StatusCode(503, "No puzzles available from API");
    }
    int randomNumber = Random.Shared.Next(0,21);

    return Ok(response[randomNumber]);
    }
}