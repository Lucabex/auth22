namespace auth22.Models;

public class User
{
    public int Id{get;set;}
    public string? UserName{get;set;}
    public string? HashedPassword{get;set;}
    public int SolvedPuzzle{get;set;}=0;
    public int Attempt{get;set;}=0;
    
}