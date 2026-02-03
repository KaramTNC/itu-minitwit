namespace Core;

/// <summary>
/// DTO for a Cheep object to be used in ViewModels
/// </summary>
/// <param name="CheepId">Uniquely identifies the Cheep object</param>
/// <param name="Author">String representation of the Cheep author's name</param>
/// <param name="Message">String information of the Cheep's message</param>
/// <param name="Timestamp">String representation of the Cheep's DateTime</param>
/// <param name="Email">String representation of the Cheep author's email</param>
/// <param name="IsFollowed">Determines if the viewing user has followed the Cheep's author</param>
/// <param name="Likes">Represents amount of likes on the Cheep</param>
/// <param name="IsLiked">Determines if the viewing user has liked this Cheep</param>
public record CheepViewModel(int CheepId, string Author, string Message, string Timestamp, string Email, 
    bool IsFollowed, List<int> Likes, bool IsLiked);

/// <summary>
/// DTO for an Author object to be used in ViewModels
/// </summary>
/// <param name="Author">String representation of the Author's name</param>
/// <param name="Email">String representation of the Author's email</param>
public record AuthorViewModel(string Author, string Email);