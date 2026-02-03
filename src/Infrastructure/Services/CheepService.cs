using System.Globalization;
using Core;
using Core.Interfaces;
using Core.Model;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

/// <summary>
/// Application Service interacting with Repositories
/// </summary>
public class CheepService : ICheepService
{
    private readonly CheepRepository _cheepRepository;
    private readonly AuthorRepository _authorRepository;

    public CheepService(ChatDbContext dbContext)
    {
        _authorRepository = new AuthorRepository(dbContext);
        _cheepRepository = new CheepRepository(dbContext);
    }

    public async Task<List<Cheep>> GetCheeps(int page)
    {
        return await _cheepRepository.ReadCheeps(page);
    }

    public async Task<List<Cheep>> GetCheepsFromAuthorId(int authorId, int page)
    {
        // filter by the provided author name
        return await _cheepRepository.GetAuthorCheeps(authorId);
    }

    public async Task<List<Cheep>> GetCheepsFromFollowed(List<int> follows, int page = 0)
    {
        // filter by the provided author name
        return await _cheepRepository.ReadCheepsFollowed(follows, page);
    }


    public async Task<Author> GetAuthorFromName(string authorName, int page)
    {
        return await _authorRepository.ReturnBasedOnNameAsync(authorName, page);
    }

    public async Task<Author> GetAuthorFromEmail(string authorEmail, int page)
    {
        var author = await _authorRepository.ReturnBasedOnEmailAsync(authorEmail, page);
        
        return author.First();
    }


    public async Task<int> GetAuthorId(string email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email) + " is null");
        }
        return await _authorRepository.ReturnAuthorsId(email);
    }

    public async Task<Author?> GetEmail(string email, int page)
    {
        var result = await _authorRepository.ReturnBasedOnEmailAsync(email, page);
        
        try
        {
            return result[0];
        }
        catch
        {
            return null;
        }
    }


    public async Task<List<int>> GetFollowers(string email)
    {
        return await _authorRepository.ReturnFollowAuthorsIds(email);
    }

    public async Task CreateCheep(string email, string msg)
    {
        var author = await _authorRepository.ReturnBasedOnEmailAsync(email);

        if (author.Count() == 1)
        {
            await _cheepRepository.CreateCheep(author[0], msg);
        } 
    }

    public void CreateAuthor(string author, string email)
    {
        _authorRepository.CreateAuthor(author, email);
    }


    public void AddFollowerId(Author author, int id)
    {
        _authorRepository.AddFollowerId(author, id);
    }

    public void RemoveFollowerId(Author author, int id)
    {
        _authorRepository.RemoveFollowerId(author, id);
    }

    public async Task DeleteAuthor(string email)
    {
        var authorId = await _authorRepository.ReturnAuthorsId(email);
        var likedCheepIds = await _authorRepository.GetLikedCheeps(email);

        // Remove liked cheeps from the deleted author
        var likedCheeps = new List<Cheep>();
        foreach (var id in likedCheepIds)
        {
            var item = await _cheepRepository.GetCheepFromId(id);
            if (item != null)
            {
                likedCheeps.Add(item);
            }
        }

        foreach (var cheep in likedCheeps)
        {
            _cheepRepository.RemovelikedId(cheep, authorId);
        }

        // Delete authors cheeps
        var authorCheeps = await _cheepRepository.GetAuthorCheeps(authorId);
        foreach (var cheep in authorCheeps)
        {
            await _cheepRepository.DeleteCheep(cheep);
        }

        // Delete author
        await _authorRepository.DeleteAuthor(email);
    }

    public async Task<List<CheepViewModel>> GetAllCheeps(string? name, string? userEmail, int page) //user, page
    {
        var cheeps = new List<CheepViewModel>();
        var result = await _cheepRepository.ReadCheeps(page);

        int? userId = null;
        
        if (!string.IsNullOrEmpty(userEmail))
        {
            try
            {
                userId = await GetAuthorId(userEmail);
            }
            catch (Exception)
            {
                // User email doesn't exist as author yet, continue without userId
                userId = null;
            }
        }
        
        List<int>? followerIds = null;
        if (name != null && userEmail != null)
        {
            userId = await GetAuthorId(userEmail);
            followerIds = await GetFollowers(userEmail);
        }
        
        foreach (var cheep in result)
        {
            cheeps.Add(await BuildCheepViewModel(cheep, userId, followerIds));
        }

        return cheeps;
    }

    public async Task UpdateFollower(string userEmail, string followerEmail)
    {
        var followerId = await _authorRepository.ReturnAuthorsId(followerEmail);
        var author = await GetEmail(userEmail, 0);
        var followers = await GetFollowers(userEmail);

        if (author != null)
        {
            if (followers.Contains(followerId))
            {
                _authorRepository.RemoveFollowerId(author, followerId);
            }
            else
            {
                _authorRepository.AddFollowerId(author, followerId);
            }
        }
    }

    public async Task<List<CheepViewModel>> GetUserTimelineCheeps(string? userEmail, Author userTimelineAuthor, int page)
    {
        var userId = -1;
        List<int> followerIds = new();

        if (userEmail != null)
        {
            userId = await GetAuthorId(userEmail);
            followerIds = await GetFollowers(userEmail);
        }
        List<Cheep> cheepsList;
        bool isOwnTimeline = userEmail != null && userTimelineAuthor.Email == userEmail;
        
        if (isOwnTimeline)
        {
            followerIds.Add(userId);
            cheepsList = await GetCheepsFromFollowed(followerIds, page);
        }
        else
        {
            cheepsList = await GetCheepsFromAuthorId(userTimelineAuthor.AuthorId, page);
        }

        // Add all cheeps into a CheepViewModel and return it
        var cheeps = new List<CheepViewModel>();
        foreach (var cheep in cheepsList)
        {
            cheeps.Add(await BuildCheepViewModel(cheep, userId, followerIds));
        }

        return cheeps;
    }


    public async Task<List<CheepViewModel>> GetUserCheeps(string userEmail, int page)
    {
        var author = await GetAuthorFromEmail(userEmail, 0);
        var userCheeps = await GetCheepsFromAuthorId(author.AuthorId, page);
        var cheeps = new List<CheepViewModel>();
        foreach (var cheep in userCheeps)
        {
            cheeps.Add(await BuildCheepViewModel(cheep, author.AuthorId));
        }

        return cheeps;
    }

    public async Task<AuthorViewModel> GetAuthorViewModel(string email)
    {
        var author = await GetAuthorFromEmail(email, 0);
        var authorViewModel = new AuthorViewModel(author.Name, author.Email);

        return authorViewModel;
    }

    public async Task<List<AuthorViewModel>> GetFollowerViewModel(string email)
    {
        var followerIds = await GetFollowers(email);
        var followerViewModels = new List<AuthorViewModel>();

        var followers = await _authorRepository.GetAuthorsFromIdList(followerIds);
        foreach (var followerAuthor in followers)
        {
            followerViewModels.Add(new AuthorViewModel(followerAuthor.Name, followerAuthor.Email));
        }

        return followerViewModels;
    }

    public async Task UpdateCheepLikes(int cheepId, string userEmail)
    {
        var cheep = await _cheepRepository.GetCheepFromId(cheepId);
        var author = await GetEmail(userEmail, 0);
        var cheepLikes = await _cheepRepository.GetLikedAuthors(cheepId);
        if (author != null && cheep != null)
        {
            foreach (int t in cheepLikes)
            {
                if (author.AuthorId == t)
                {
                    _cheepRepository.RemovelikedId(cheep, author.AuthorId);
                    _authorRepository.RemoveLikeId(author, cheepId);
                    return;
                }
            }
            
            _cheepRepository.AddlikedId(cheep, author.AuthorId);
            _authorRepository.AddLikeId(author, cheepId);
        }
    }

    public async Task<List<int>> GetCheepLikesAmount(int cheepId)
    {
        return await _cheepRepository.GetLikedAuthors(cheepId);
    }

    public async Task<List<CheepViewModel>> GetLikedCheepsForAuthor(string userEmail)
    {
        var author = await GetAuthorFromEmail(userEmail, 0);

        var likedCheepIds = await _authorRepository.GetLikedCheeps(userEmail);
        var likedCheeps = new List<CheepViewModel>();
        var followerIds = await GetFollowers(userEmail);
        
        foreach (var cheepId in likedCheepIds)
        {
            var cheep = await _cheepRepository.GetCheepFromId(cheepId);
            if (cheep == null)
            {
                _authorRepository.RemoveLikeId(author, cheepId);
                continue;
            }
            
            likedCheeps.Add(await BuildCheepViewModel(cheep, author.AuthorId, followerIds));
        }

        return likedCheeps;
    }
    
    
    private async Task<CheepViewModel> BuildCheepViewModel(
        Cheep cheep,
        int? userId = null,
        List<int>? followerIds = null)
    {
        var isFollowed = followerIds?.Contains(cheep.Author.AuthorId) ?? false;
        var isLiked = userId.HasValue && cheep.PeopleLikes.Contains(userId.Value);
        var likes = await GetCheepLikesAmount(cheep.CheepId);

        return new CheepViewModel(
            cheep.CheepId,
            cheep.Author.Name,
            cheep.Text,
            cheep.TimeStamp.ToString(CultureInfo.CurrentCulture),
            cheep.Author.Email,
            isFollowed,
            likes,
            isLiked
        );
    }
}