using Core.Model;

namespace Core.Interfaces;


/// <summary>
/// Interface for the AuthorRepository that represents the Author table
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Create a new entry in the Author table
    /// </summary>
    /// <param name="name">Author name</param>
    /// <param name="email">Author email</param>
    public void CreateAuthor(string name, string email);
    
    /// <summary>
    /// Add a new followingId under an Author entry
    /// </summary>
    /// <param name="author">Author entry object</param>
    /// <param name="id">Following id</param>
    public void AddFollowerId(Author author, int id);
    
    /// <summary>
    /// Remove an existing followingId under an Author entry
    /// </summary>
    /// <param name="author">Authoe entry object</param>
    /// <param name="id">following id</param>
    public void RemoveFollowerId(Author author, int id);
    
    /// <summary>
    /// Check the Author table for a uniquely unoccupied Author ID entry
    /// </summary>
    /// <returns>Integer representing an unoccupied entry ID</returns>
    public Task<int> FindNewAuthorId();
    
    /// <summary>
    /// Query the Author table for a list of entries matching an Email
    /// </summary>
    /// <param name="email">Author entry's email</param>
    /// <param name="page">Page of the query</param>
    /// <returns>List of Author entries matching the email</returns>
    public Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0);
    
    /// <summary>
    /// Query the Author table for a list of entry IDs matching an Email
    /// </summary>
    /// <param name="email">Email to match IDs</param>
    /// <returns>List of ID entries matching the Email</returns>
    public Task<List<int>> ReturnFollowAuthorsIds(string email);

    
    /// <summary>
    /// Query the Author table for an ID matching an Email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<int> ReturnAuthorsId(string email);

    /// <summary>
    /// Query the Author table for an Author entry matching a string name
    /// </summary>
    /// <param name="name">String name of the entry</param>
    /// <param name="page">Page of the query</param>
    /// <returns></returns>
    public Task<Author> ReturnBasedOnNameAsync(string name, int page = 0);
    
    /// <summary>
    /// Delete a table entry matching the email
    /// </summary>
    /// <param name="email">Entry matching the email</param>
    /// <returns></returns>
    public Task DeleteAuthor(string email);

    /// <summary>
    /// Query for a list of Author entries matching a list of entry IDs
    /// </summary>
    /// <param name="idList">List of author IDs</param>
    /// <returns>List of Author entries matching idList</returns>
    public Task<List<Author>> GetAuthorsFromIdList(List<int> idList);
    
    /// <summary>
    /// Query for a list of an Author entry's Liked Cheeps Ids
    /// </summary>
    /// <param name="email">Author's email</param>
    /// <returns>List of Liked Cheeps IDs</returns>
    public Task<List<int>> GetLikedCheeps(string email);

    /// <summary>
    /// Insert a cheepID into an Author entry's list of LikedCheeps
    /// </summary>
    /// <param name="author">Author object</param>
    /// <param name="cheepId">Cheep Id</param>
    public void AddLikeId(Author author, int cheepId);

    /// <summary>
    /// Remove a CheepID into an Author entry's list of LikedCheeps
    /// </summary>
    /// <param name="author">Author object</param>
    /// <param name="cheepId">Cheep Id</param>
    public void RemoveLikeId(Author author, int cheepId);
    
    /// <summary>
    /// Query the Author table for whether an id is available
    /// </summary>
    /// <param name="authorId">A candidate id</param>
    /// <returns>Whether the id is available or not</returns>
    public Task<bool> CheckIfAuthorIdIsAvailable(int authorId);
}