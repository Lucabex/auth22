using auth22.Models;
using Microsoft.EntityFrameworkCore;
namespace auth22.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User>User{get;set;}
}
