using Core.Interfaces;
using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository representing the Cheeps table
/// </summary>
public class CheepRepository : ICheepRepository
{
    private readonly ChatDbContext _dbContext;
    public CheepRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task CreateCheep(Author author, string msg)
    {
       var cheep = new Cheep()
        {
            CheepId = FindNewCheepId(),
            Text = msg,
            TimeStamp = DateTime.Now,
            AuthorId = author.AuthorId,
            Author = author,
        };

        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
    
    public int FindNewCheepId()
    {
        return _dbContext.Cheeps.Count() + 1;
    }

    public async Task<List<Cheep>> ReadCheeps(int page = 0)
    {
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            select cheep).OrderByDescending(c => c.TimeStamp).Skip(page*32).Take(32);
        
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> ReadCheepsPerson(string name, int page)
    {
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            where cheep.Author.Name == name
            select cheep
            ).OrderByDescending(c => c.TimeStamp).Skip(page * 32).Take(32);
        var result = await query.ToListAsync();

        
        return result;
    }

    public async Task<List<Cheep>> ReadCheepsFollowed(List<int> follows, int page)
    {
        
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            where follows.Contains(cheep.Author.AuthorId)
            select cheep
            ).OrderByDescending(c => c.TimeStamp).Skip(page * 32).Take(32);
        var result = await query.ToListAsync();

        
        return result;
    }

    public async Task<List<int>> GetLikedAuthors(int cheepId)
    {
        var query = (
            from cheep in _dbContext.Cheeps
            where cheep.CheepId == cheepId
            select cheep.PeopleLikes
        );

        var result = await query.ToListAsync();

        try
        {
            return result[0];
        }
        catch
        {
            return new List<int>();
        }
    }

    
    
    
    public void AddlikedId(Cheep cheep, int authorId)
    {
        cheep.PeopleLikes.Add(authorId);
        _dbContext.Update(cheep);
        _dbContext.SaveChanges();

    }

    public void RemovelikedId(Cheep cheep, int authorId)
    {
        cheep.PeopleLikes.Remove(authorId);
        _dbContext.Update(cheep);
        _dbContext.SaveChanges();

    }

    public async Task<Cheep?> GetCheepFromId(int cheepId)
    {
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            where cheep.CheepId == cheepId
            select cheep
        );

        var returnList = await query.ToListAsync();

        try
        {
            return returnList[0];
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Cheep>> GetAuthorCheeps(int authorId)
    {
        var query = (
            from cheep in _dbContext.Cheeps
            where cheep.AuthorId == authorId 
            select cheep
        );
        
        var returnList = await query.ToListAsync();
        return returnList;
    }

    public async Task DeleteCheep(Cheep cheep)
    {
        _dbContext.Remove(cheep);
        await _dbContext.SaveChangesAsync();
    }
    
}