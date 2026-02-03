using Core.Interfaces;
using Core.Model;
using SupportScripts;

namespace UnitTest;

public class CheepRepositoryTests
{
    private readonly MemoryDbFactory _memoryDb = new MemoryDbFactory();
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepositoryTests()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        _cheepRepository = _memoryDb.GetCheepRepository();
        _authorRepository = _memoryDb.GetAuthorRepository();
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(_cheepRepository);
    }

    [Fact]
    public void FindNewIdTest()
    {
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
    }


    /*
    The test below was just to verify we could add to the new data base;
    The test case should be updated with the queries to do the following instead of what is is currently
    doing.

    Assert.False(query author)
    add author
    Assert.True(query author)
    */

    [Fact]
    public void DoesItCreateAuthor()
    {
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
        _authorRepository.CreateAuthor("Tim", "tim@email.com");
        Assert.Equal(4, _authorRepository.FindNewAuthorId().Result);
    }


    [Fact]
    public async Task CreateCheep_WithExistingAuthor()
    {
        var author = "Helge";
        var email = "ropf@itu.dk";
        var msg = "HELLO WORLD!";

        var authorObj = new Author()
        {
            Email = email,
            Name = author,
            AuthorId = 0,
            Cheeps = new List<Cheep>()
        };

        //Values needs to be updated when merged with Vee and madelines code
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
        Assert.Equal(5, _cheepRepository.FindNewCheepId());
        await _cheepRepository.CreateCheep(authorObj, msg);
        Assert.Equal(4, _authorRepository.FindNewAuthorId().Result);
        Assert.Equal(6, _cheepRepository.FindNewCheepId());
    }
    
    [Fact]
    public async Task DoesItAddCheepLikeId()
    {
        var cheep = await _cheepRepository.GetCheepFromId(2);
        var likedAuthors = await _cheepRepository.GetLikedAuthors(2);
        
        Assert.NotNull(likedAuthors);
        Assert.Empty(likedAuthors);
        _cheepRepository.AddlikedId(cheep!, 1);
        Assert.Equal(1, cheep!.PeopleLikes[0]);


    }

    [Fact]
    public async Task DoesItRemoveCheepLikeId()
    {
        var cheep = await _cheepRepository.GetCheepFromId(4);
        var likedAuthors = await _cheepRepository.GetLikedAuthors(4);
        
        Assert.NotNull(likedAuthors);
        Assert.Equal(1, cheep!.PeopleLikes[0]);
        _cheepRepository.RemovelikedId(cheep!, 1);
        likedAuthors = await _cheepRepository.GetLikedAuthors(4);
        Assert.Empty(likedAuthors);


    }

    [Fact]
    public async Task DoesItDeleteCheep()
    {
        var cheep = await _cheepRepository.GetCheepFromId(4);
        await _cheepRepository.DeleteCheep(cheep!);
        cheep = await _cheepRepository.GetCheepFromId(4);
        Assert.Null(cheep);
    }

    [Fact]
    public async Task DoesItAddFollowerId()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Adrian");
        Assert.NotNull(author.Follows);
        Assert.Empty(author.Follows);
        _authorRepository.AddFollowerId(author, 1);
        Assert.Equal(1, author.Follows[0]);

    }

    [Fact]
    public async Task DoesItRemoveFollowerId()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");
        Assert.NotNull(author.Follows);
        Assert.Equal(2, author.Follows[0]);
        _authorRepository.RemoveFollowerId(author, 2);
        Assert.Empty(author.Follows);

    }


    [Fact]
    public async Task DoesItRemoveAuthorLikeId()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Adrian");
        Assert.Equal(1, author.CheepLikes[0]);
        _authorRepository.RemoveLikeId(author, 1);
        Assert.Empty(author.CheepLikes);

    }

    [Fact]
    public async Task DoesItAddAuthorLikeId()
    {
         var author = await _authorRepository.ReturnBasedOnNameAsync("Adrian");
        Assert.Single(author.CheepLikes);
        _authorRepository.AddLikeId(author, 2);
        Assert.Equal(2, author.CheepLikes.Count);
        Assert.Equal(2, author.CheepLikes[1]);
    }

}