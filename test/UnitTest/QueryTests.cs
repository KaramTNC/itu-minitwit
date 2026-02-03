using Core.Interfaces;
using Core.Model;
using SupportScripts;

namespace UnitTest;

public class QueryTests
{
    private readonly MemoryDbFactory _memoryDb = new MemoryDbFactory();
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;


    public QueryTests()
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
    public async Task ReadCheeps()
    {
        var cheeps = await _cheepRepository.ReadCheeps(0);

        var testCheep = cheeps.Find(x => x.Author.Name == "Helge");
        
        Assert.NotNull(testCheep);
        Assert.Equal(  "Helge"  , testCheep.Author.Name  );
    }




    [Fact]
    public async Task ReadCheepsAuthor()
    {
        var cheeps = await _cheepRepository.ReadCheepsPerson("Helge", 0);

        var testCheep = cheeps.Find(x => x.Author.Name == "Helge");
        Assert.NotNull(testCheep);
        Assert.Equal("Madeleine says i make propaganda", testCheep.Text);

        Assert.Null(cheeps.Find(x => x.Author.Name == "Adrian"));
    }

    
    

    [Fact]
    public async Task ReadAuthor()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");

        Assert.Equal("ropf@itu.dk", author.Email);
    }

    [Fact]
    public async Task ReadEmail()
    {
        var author = await _authorRepository.ReturnBasedOnEmailAsync("ropf@itu.dk");

        Assert.Equal("Helge", author[0].Name);
    }

    [Fact]
    public async Task ReadCheepsFollowing()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");

        var follows = await _authorRepository.ReturnFollowAuthorsIds(author.Email);
        Assert.NotNull(follows);

        var cheeps = await _cheepRepository.ReadCheepsFollowed(follows, 0);
        var testCheep = cheeps.Find(x => x.Author.Name == "Adrian");
        Assert.NotNull(testCheep);
        Assert.Equal("test answer", testCheep.Text);
        Assert.Null(cheeps.Find(x => x.Author.Name == "Helge"));

    }

    [Fact]
    public async Task ReadLikedAuthors()
    {
        var likedAuthors = await _cheepRepository.GetLikedAuthors(3);

        Assert.NotNull(likedAuthors);

        Assert.Equal(1, likedAuthors[0]);

    }

    [Fact]
    public async Task ReadCheepFromId()
    {
        var cheep = await _cheepRepository.GetCheepFromId(1);

        Assert.Equal("Join itu lan now", cheep!.Text);
    }


    [Fact]
    public async Task ReadAuthorCheeps()
    {
        var cheeps = await _cheepRepository.GetAuthorCheeps(2);
        Assert.NotNull(cheeps[0]);
        Assert.Equal("test answer", cheeps[0].Text);
    }

    
    [Fact]
    public async Task ReadFollowAuthorsIds()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");
        var follows = await _authorRepository.ReturnFollowAuthorsIds(author.Email);
        Assert.NotNull(follows);
        Assert.Equal(2, follows[0]);
        
    }

    [Fact]
    public async Task ReadAuthorId()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");
        var id = await _authorRepository.ReturnAuthorsId(author.Email);
        Assert.Equal(1, id);
    }

    [Fact]
    public async Task ReadAuthorsFromIdList()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");
        var author2 = await _authorRepository.ReturnBasedOnNameAsync("Adrian");
        var follows = await _authorRepository.ReturnFollowAuthorsIds(author.Email);

        var authorFollows = await _authorRepository.GetAuthorsFromIdList(follows);
        Assert.NotNull(authorFollows);
        Assert.Equal(author2, authorFollows[0]);
    }

    [Fact]
    public async Task ReadLikedCheeps()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Adrian");
        var cheepLikes = await _authorRepository.GetLikedCheeps(author.Email);
        Assert.NotNull(cheepLikes);
        Assert.Equal(1, cheepLikes[0]);
    }
    
}