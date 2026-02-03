using Core.Interfaces;
using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository representing the Author table
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ChatDbContext _dbContext;
    public AuthorRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void CreateAuthor(string name, string email)
    {
        var newAuthor = new Author()
        {
            AuthorId = FindNewAuthorId().Result,
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };

        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    public void AddFollowerId(Author author, int id)
    {
        author.Follows.Add(id);
        _dbContext.Update(author);
        _dbContext.SaveChanges();
    }

    public void RemoveFollowerId(Author author, int id)
    {
        author.Follows.Remove(id);
        _dbContext.Update(author);
        _dbContext.SaveChanges();

    }
    
    public async Task DeleteAuthor(string email)
    {
        //var usersList = _dbContext.Users;
        var myUser = _dbContext.Users.SingleOrDefault(user => user.NormalizedEmail == email.ToUpper());
        var author = await ReturnBasedOnEmailAsync(email);

        if (author.Count() != 1 || author.Count == 0) return;
        
        //await _dbContext.Users.FindAsync(email);
        _dbContext.Authors.Remove(author.First());
        _dbContext.Users.Remove(myUser!);
        
        await _dbContext.SaveChangesAsync();
    }

    #region Helper methods

    public async Task<int> FindNewAuthorId()
    {
        var length = _dbContext.Authors.Count();
        var newId = length + 1;

        var idExists = await CheckIfAuthorIdIsAvailable(newId);
        while (idExists == false)
        {
            Console.WriteLine("CURRENT ID TO CHECK:" + newId);
            newId++;
            idExists = await CheckIfAuthorIdIsAvailable(newId);
        }

        return newId;
    }

    #endregion
    
    public async Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person
            ).OrderByDescending(c => c.Name).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        return result;
    }
    
    public async Task<List<int>> ReturnFollowAuthorsIds(string email)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person.Follows
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

    public async Task<int> ReturnAuthorsId(string email)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person.AuthorId
        );
        var result = await query.ToListAsync();

        if (result.Count == 0)
        {
            throw new Exception("No authors found");
        }

        return result[0];
    }

    public async Task<Author> ReturnBasedOnNameAsync(string name, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Name == name
            select person
            ).OrderByDescending(c => c.Name).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();
        
        if (result.Count == 0) throw new Exception("No authors found from " + name);

        return result[0];
    }
    
    public async Task<List<Author>> GetAuthorsFromIdList(List<int> idList)
    {
        var query = (
            from author in _dbContext.Authors
            where idList.Contains(author.AuthorId)
            select author
        ).OrderByDescending(c => c.Name);
        var result = await query.ToListAsync();

        return result;
    }

    //Has test
    public async Task<bool> CheckIfAuthorIdIsAvailable(int authorId)
    {
        var query = (
            from author in _dbContext.Authors
            where authorId == author.AuthorId
            select author
        ).OrderByDescending(c => c.Name);
        var result = await query.ToListAsync();

        if (result.Count == 0) return true;
        return false;
    }

    public async Task<List<int>> GetLikedCheeps(string email)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person.CheepLikes
        );
        
        var result = (await query.ToListAsync())[0];
        
        try
        {
            return result;
        }
        catch
        {
            return new List<int>();
        }
    }
    
    //Has test
    public void RemoveLikeId(Author author, int cheepId)
    {
        author.CheepLikes.Remove(cheepId);
        _dbContext.Update(author);
        _dbContext.SaveChanges();
    }
    
    //Has test
    public void AddLikeId(Author author, int cheepId)
    {
        author.CheepLikes.Add(cheepId);
        _dbContext.Update(author);
        _dbContext.SaveChanges();
    }
}