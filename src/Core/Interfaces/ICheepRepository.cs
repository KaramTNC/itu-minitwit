using Core.Model;

namespace Core.Interfaces;

/// <summary>
/// Interface for the CheepRepository that represents the Cheep table
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Insert a new Cheep entry for an Author and its message
    /// </summary>
    /// <param name="author">Author object</param>
    /// <param name="msg">Cheep message</param>
    /// <returns></returns>
    Task CreateCheep(Author author, string msg);
    
    /// <summary>
    /// Query the table for a list of Cheeps filtered by page
    /// </summary>
    /// <param name="page">Which page to get</param>
    /// <returns>List of Cheeps</returns>
    Task<List<Cheep>> ReadCheeps(int page);
    
    /// <summary>
    /// Query the table for a list of Cheeps from a specific author name
    /// </summary>
    /// <param name="name">String representation of a Cheep's author</param>
    /// <param name="page">Whic page to return</param>
    /// <returns>List of Cheeps matching the author name</returns>
    Task<List<Cheep>> ReadCheepsPerson(string name, int page);
    
    /// <summary>
    /// Query the table for a list of Cheeps from a list of author IDs
    /// </summary>
    /// <param name="follows"></param>
    /// <param name="page"></param>
    /// <returns>List of Cheeps matching the list of author IDs</returns>
    Task<List<Cheep>> ReadCheepsFollowed(List<int> follows, int page);
    
    /// <summary>
    /// Query the table for an available Cheep id
    /// </summary>
    /// <returns>Unoccupied Cheep Id</returns>
    public int FindNewCheepId();
    
    /// <summary>
    /// Query for a list of Author IDs who liked a given Cheep ID
    /// </summary>
    /// <param name="cheepId">Cheep ID to query for</param>
    /// <returns>List of Author IDs that have liked this cheep</returns>
    public Task<List<int>> GetLikedAuthors(int cheepId);

    /// <summary>
    /// Insert an authorID into a Cheep entry's list of Liked IDs
    /// </summary>
    /// <param name="cheep">Cheep object entry</param>
    /// <param name="authorId">Author ID that has liked the cheep</param>
    public void AddlikedId(Cheep cheep, int authorId);

    /// <summary>
    /// Delete a liked authorID from a Cheep entry
    /// </summary>
    /// <param name="cheep">Cheep object entry to delete from</param>
    /// <param name="authorId">AuthorID to delete</param>
    public void RemovelikedId(Cheep cheep, int authorId);

    /// <summary>
    /// Query for a Cheep entry given a matching CheepID
    /// </summary>
    /// <param name="cheepId">CheepID to match for</param>
    /// <returns>Cheep object entry</returns>
    public Task<Cheep?> GetCheepFromId(int cheepId);

    /// <summary>
    /// Query for a list of Cheep entries belonging to an AuthorID
    /// </summary>
    /// <param name="authorId">AuthorID to match for</param>
    /// <returns>List of Cheeps belonging to an AuthorID</returns>
    public Task<List<Cheep>> GetAuthorCheeps(int authorId);

    /// <summary>
    /// Delete a Cheep entry given a Cheep object
    /// </summary>
    /// <param name="cheep">Cheep object to delete</param>
    /// <returns></returns>
    public Task DeleteCheep(Cheep cheep);
}
