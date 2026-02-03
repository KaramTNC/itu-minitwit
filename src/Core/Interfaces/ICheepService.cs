using Core.Model;

namespace Core.Interfaces;

/// <summary>
/// Interface for CheepService for the ViewModels
/// </summary>
public interface ICheepService
{
    /// <summary>
    /// Get a List of all Cheeps filtered by pages
    /// </summary>
    /// <param name="page">Page to look up</param>
    /// <returns>List of Cheeps</returns>
    public Task<List<Cheep>> GetCheeps(int page);
    
    /// <summary>
    /// Get a List of All Cheeps from a specific Author ID
    /// </summary>
    /// <param name="authorId">AuthorID to fetch for</param>
    /// <param name="page">Page to look up</param>
    /// <returns>List of Cheeps filered by Author ID</returns>
    public Task<List<Cheep>> GetCheepsFromAuthorId(int authorId, int page);
    
    /// <summary>
    /// Get a List of Cheeps from a list of Author IDs
    /// </summary>
    /// <param name="follows">List of Author IDs to fetch from</param>
    /// <param name="page">Page to look up</param>
    /// <returns>List of Cheeps filtered by list of Author IDs</returns>
    public Task<List<Cheep>> GetCheepsFromFollowed(List<int> follows, int page);
    
    /// <summary>
    /// Get an Author object from a specific authorName
    /// </summary>
    /// <param name="authorName">String authorName to fetch from</param>
    /// <param name="page">Page to lookup</param>
    /// <returns>Author object</returns>
    public Task<Author> GetAuthorFromName(string authorName, int page);
    
    /// <summary>
    /// Get an AuthorID based on a matching email
    /// </summary>
    /// <param name="email">String email to fetch from</param>
    /// <returns>Integer id for an author</returns>
    public Task<int> GetAuthorId(string email);
    
    /// <summary>
    /// Get an Author object if it exists, from a specific email
    /// </summary>
    /// <param name="email">Email to get from</param>
    /// <param name="page">Page to look up</param>
    /// <returns>Author object</returns>
    public Task<Author?> GetEmail(string email, int page);
    
    /// <summary>
    /// Get a list of AuthorIDs that a specified Author follows via its email
    /// </summary>
    /// <param name="email">Author's email to fetch from</param>
    /// <returns>List of AuthorIDs that have been followed</returns>
    public Task<List<int>> GetFollowers(string email);
    
    /// <summary>
    /// Create a Cheep object given an author's email and their message
    /// </summary>
    /// <param name="email">Author email</param>
    /// <param name="msg">String message</param>
    /// <returns></returns>
    public Task CreateCheep(string email, string msg);
    
    /// <summary>
    /// Create a new Author given a name and email
    /// </summary>
    /// <param name="author">Author name</param>
    /// <param name="email">Author email</param>
    public void CreateAuthor(string author, string email);
    
    /// <summary>
    /// Add a new authorID to a specific Author's list of Following
    /// </summary>
    /// <param name="author">Author object to add to</param>
    /// <param name="id">authorID to add</param>
    public void AddFollowerId(Author author, int id);
    
    /// <summary>
    /// Remove an authorID from a specific Author's list of Following
    /// </summary>
    /// <param name="author">Author object to remove from</param>
    /// <param name="id">authorID to remove</param>
    public void RemoveFollowerId(Author author, int id);
    
    /// <summary>
    /// Delete an Author from the database given its email
    /// </summary>
    /// <param name="email">Author email</param>
    /// <returns></returns>
    public Task DeleteAuthor(string email);

    /// <summary>
    /// Get all CheepViewModels matching a specific User
    /// </summary>
    /// <param name="name">User name</param>
    /// <param name="userEmail">User email</param>
    /// <param name="page">Page to look up</param>
    /// <returns>List of CheepViewModels for the user</returns>
    public Task<List<CheepViewModel>> GetAllCheeps(string name, string userEmail, int page);

    /// <summary>
    /// Update the user's following list given an author's email
    /// </summary>
    /// <param name="userEmail">User's email to update with</param>
    /// <param name="followerEmail">Author's email to fetch from</param>
    /// <returns></returns>
    public Task UpdateFollower(string userEmail, string followerEmail);

    /// <summary>
    /// Get a List of CheepViewModels for a UserTimeline
    /// </summary>
    /// <param name="userEmail">The current User's email</param>
    /// <param name="userTimelineAuthor">The UserTimeline's Author</param>
    /// <param name="page">Page to lookup</param>
    /// <returns>List of CheepViewModels</returns>
    public Task<List<CheepViewModel>> GetUserTimelineCheeps(string userEmail, Author userTimelineAuthor, int page);
    
    /// <summary>
    /// Get a list of CheepViewModels for the current User
    /// </summary>
    /// <param name="userEmail">User's email</param>
    /// <param name="page">Page to look up</param>
    /// <returns>List of CheepViewModels for the user</returns>
    public Task<List<CheepViewModel>> GetUserCheeps(string userEmail, int page);

    /// <summary>
    /// Get an AuthorViewModel for a specific author using their Email
    /// </summary>
    /// <param name="email">Author's email</param>
    /// <returns>AuthorViewModel representing the Author</returns>
    public Task<AuthorViewModel> GetAuthorViewModel(string email);

    /// <summary>
    /// Get a List of AuthorViewModels for a specific author's following list using their email
    /// </summary>
    /// <param name="email">Author's email's to get from</param>
    /// <returns>List of Author Following's AuthorViewModels</returns>
    public Task<List<AuthorViewModel>> GetFollowerViewModel(string email);

    /// <summary>
    /// Update a Cheep's LikedIDs using the User's email
    /// </summary>
    /// <param name="cheepId">CheepID to be updated</param>
    /// <param name="userEmail">User's email</param>
    /// <returns></returns>
    public Task UpdateCheepLikes(int cheepId, string userEmail);
    
    /// <summary>
    /// Get the Liked List of authorIDs that a Cheep contains
    /// </summary>
    /// <param name="cheepId">CheepID to check from</param>
    /// <returns>List of LikeIDs</returns>
    public Task<List<int>> GetCheepLikesAmount(int cheepId);
    
    /// <summary>
    /// Get a List of CheepViewModel for the User's Liked Cheeps
    /// </summary>
    /// <param name="userEmail">User's email</param>
    /// <returns>List of Liked CheepViewModel</returns>
    public Task<List<CheepViewModel>> GetLikedCheepsForAuthor(string userEmail);
    
    /// <summary>
    /// Get an Author matching their email
    /// </summary>
    /// <param name="authorEmail">Author email to match for</param>
    /// <param name="page">Page to look up</param>
    /// <returns>Author object</returns>
    public Task<Author> GetAuthorFromEmail(string authorEmail, int page);
}