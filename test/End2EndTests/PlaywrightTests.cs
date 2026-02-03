using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.None)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    private string ServerAddress => GlobalTestSetup.ServerAddress;
    private IPage _page;

    [SetUp]
    public async Task Setup()
    {
        var context = await GlobalTestSetup.Browser.NewContextAsync();
        _page = await context.NewPageAsync();
        await _page.GotoAsync(ServerAddress);
    }

    [TearDown]
    public async Task Teardown()
    {
        await _page.Context.CloseAsync(); // clean up context after each test
    }

    [Test]
    public async Task BasicTest()
    {
        var response = await _page.GotoAsync(GlobalTestSetup.ServerAddress);
        Assert.That(response!.Status, Is.EqualTo(200));
    }


    [Test]
    public async Task RegisterTest()
    {
        var email = "newAccount@test.com";
        var password = "Test12345!";
        var username = "newAccount";

        await RegisterAccountIdentity(email, password, username);

        await _page.GotoAsync(ServerAddress);

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/?"));
        //await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Logout" })).ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Login" })).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginLogoutIdentityTest_AccountDoesNotExist()
    {
        var email = "fakemail@test.com";
        var password = "123456";

        await LoginAccountIdentity(email, password);

        //Assert
        await Expect(_page.GetByRole(AriaRole.Alert)).ToMatchAriaSnapshotAsync("- listitem: User not found.");
    }


    [Test]
    public async Task DeleteAccount_AccountGetsDeleted()
    {
        var username = "DELETEME";
        var email = "DELETEME@test.com";
        var password = "123456789Ab.";

        await RegisterAccountIdentity(email, password, username);

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Login" }))
            .Not.ToBeVisibleAsync();

        await _page.Locator("#Cheep_Text").ClickAsync();
        await _page.Locator("#Cheep_Text").FillAsync("DELETE MY CHIRP");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await DeleteAccountIdentity();

        await Expect(_page.Locator("body")).ToMatchAriaSnapshotAsync(
            "- link \"public timeline\":\n  - /url: /\n- text: \"|\"\n- list:\n  - listitem:\n    - link \"Register\":\n      - /url: /Identity/Account/Register\n  - listitem:\n    - link \"Login\":\n      - /url: /Identity/Account/Login");

        var cheep = _page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        Assert.That(username != authorName);
    }


    [Test]
    public async Task SendCheepTest_UserSendsValidCheep()
    {
        // Arrange
        var email = "newCheepAccount@test.com";
        var password = "test123?T";
        var cheepText = "Hello im a real uwu";
        var username = "newCheepAccount";

        // Act
        await RegisterAccountIdentity(email, password, username);
        await _page.Locator("#Cheep_Text").ClickAsync();
        await _page.Locator("#Cheep_Text").FillAsync(cheepText);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        // Wait for the page to reload/update after posting
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert - Check that the first item in the message list is our new cheep
        var firstCheep = _page.Locator("#messagelist > li").First;

        // Simpler approach: verify individual elements without complex regex
        await Expect(firstCheep).ToContainTextAsync(cheepText);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveTextAsync(username);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveAttributeAsync("href", $"/{username}");
    }

    [Test]
    public async Task FollowAUser()
    {
        await RegisterAccountIdentity("follow@test.com", "test123?T", "followAccount");

        var cheep = await FollowUser("Jacqualine Gilcoine");

        await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(cheep.GetByRole(AriaRole.Link)).ToContainTextAsync("Jacqualine Gilcoine");
        var unfollowButton = cheep.GetByRole(AriaRole.Button, new() { Name = "Unfollow" });
        await Expect(unfollowButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task UnfollowAUser()
    {
        await RegisterAccountIdentity("unfollow@test.com", "test123?T", "unfollowAccount");

        // Ensure the user is followed first
        await FollowUser("Jacqualine Gilcoine");

        var cheep = await UnfollowUser("Jacqualine Gilcoine");

        var followButton = cheep.GetByRole(AriaRole.Button, new() { Name = "Follow" });
        await Expect(followButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserLikesCheep()
    {
        await RegisterAccountIdentity("likeaCheep@test.com", "test123?T", "likeAccount");

        // Locate the cheep by user
        var cheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = "Jacqualine Gilcoine" })
            .First;

        // Read current Likes count
        var likeText = await cheep.Locator(":scope >> text=Likes").InnerTextAsync();
        int currentLikes = int.Parse(Regex.Match(likeText, @"\d+").Value);

        // Perform Like
        var unlikeButton = await LikeCheep("Jacqualine Gilcoine");

        // Assert Likes increased by 1
        var newLikeText = await cheep.Locator(":scope >> text=Likes").InnerTextAsync();
        int newLikes = int.Parse(Regex.Match(newLikeText, @"\d+").Value);
        Assert.That(newLikes, Is.EqualTo(currentLikes + 1));

        // Assert button updated
        await Expect(cheep.GetByRole(AriaRole.Button, new() { Name = "Unlike" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserUnlikesCheep()
    {
        await RegisterAccountIdentity("unlikeaCheep@test.com", "test123?T", "unlikeAccount");

        // Ensure the cheep is liked first
        var cheep = await LikeCheep("Jacqualine Gilcoine");

        // Read current Likes count
        var likeText = await cheep.Locator(":scope >> text=Likes").InnerTextAsync();
        int currentLikes = int.Parse(Regex.Match(likeText, @"\d+").Value);

        // Perform Unlike
        cheep = await UnlikeCheep("Jacqualine Gilcoine");

        // Assert Likes decreased by 1
        var newLikeText = await cheep.Locator(":scope >> text=Likes").InnerTextAsync();
        int newLikes = int.Parse(Regex.Match(newLikeText, @"\d+").Value);
        Assert.That(newLikes, Is.EqualTo(currentLikes - 1));

        // Assert button updated
        await Expect(cheep.GetByRole(AriaRole.Button, new() { Name = "Like" })).ToBeVisibleAsync();
    }


    [Test]
    public async Task UserTimelineContainsUserCheeps()
    {
        var email = "timelineUser@test.com";
        var password = "Test123!";
        var username = "timelineUser";
        var message = "Hello from my timeline!";

        await RegisterAccountIdentity(email, password, username);

        // Post a cheep
        await _page.Locator("#Cheep_Text").FillAsync(message);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Go to user timeline page
        await _page.GotoAsync($"{ServerAddress}/{username}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var cheep = _page.Locator("#messagelist > li").First;
        await Expect(cheep).ToContainTextAsync(message);
        await Expect(cheep.GetByRole(AriaRole.Link).First).ToHaveTextAsync(username);
    }

    [Test]
    public async Task UserTimelineContainsFollowedCheeps()
    {
        var email = "mainUser3@test.com";
        var password = "Test123!";
        var username = "mainUser3";

        // Register main user
        await RegisterAccountIdentity(email, password, username);

        // Find the first cheep whose author is NOT the current user
        var firstOtherCheep = _page.Locator("#messagelist > li")
            .First;
    
        string authorName = await firstOtherCheep.GetByRole(AriaRole.Link).First.InnerTextAsync();
    
        if (authorName == username)
        {
            firstOtherCheep = _page.Locator("#messagelist > li").Nth(1);
            authorName = await firstOtherCheep.GetByRole(AriaRole.Link).First.InnerTextAsync();
        }

        // Follow that user
        await FollowUser(authorName);

        // Refresh user timeline
        await _page.GotoAsync($"{ServerAddress}/{username}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert the followed cheep appears on timeline
        var followedCheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = authorName })
            .First;
        await Expect(followedCheep).ToContainTextAsync(authorName);
    }

    [Test]
    public async Task UserAboutMeFollowerList_ContainsFollowers()
    {
        // Register and log in as main user
        var mainEmail = "mainUser4@test.com";
        var mainPassword = "Test123!";
        var mainUsername = "mainUser4";
        await RegisterAccountIdentity(mainEmail, mainPassword, mainUsername);

        // Wait for cheeps to appear
        await _page.WaitForSelectorAsync("#messagelist > li");

        // Find the first cheep not by main user
        var cheeps = _page.Locator("#messagelist > li");
        ILocator cheep = null;
        string authorName = null;

        int count = await cheeps.CountAsync();
        for (int i = 0; i < count; i++)
        {
            var candidate = cheeps.Nth(i);
            var link = candidate.GetByRole(AriaRole.Link).First;
            var name = await link.InnerTextAsync();

            if (name != mainUsername)
            {
                cheep = candidate;
                authorName = name;
                break;
            }
        }

        if (cheep == null)
            Assert.Inconclusive("No cheeps from other users exist to follow.");

        // Main user follows that author
        await FollowUser(authorName);

        // Go to main user's AboutMe page
        await _page.GotoAsync($"{ServerAddress}/info?page=1");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert followed author appears in main user's Following List
        var followedLink = _page.Locator("h2", new() { HasText = $"{mainUsername}'s Following List" })
            .Locator("..")
            .GetByRole(AriaRole.Link, new() { Name = authorName });

        await Expect(followedLink).ToBeVisibleAsync();
    }



    [Test]
    public async Task UserAboutMeCheepList_ContainsUserCheeps()
    {
        var email = "cheepUser@test.com";
        var password = "Test123!";
        var username = "cheepUser";
        var cheepText = "Cheep in AboutMe";

        await RegisterAccountIdentity(email, password, username);

        // Post a cheep
        await _page.Locator("#Cheep_Text").FillAsync(cheepText);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await _page.GotoAsync($"{ServerAddress}/info?page=1");
        var cheep = _page.Locator("#messagelist > li").Filter(new() { HasText = username }).First;
        await Expect(cheep).ToContainTextAsync(cheepText);
    }

    [Test]
    public async Task UserAboutMeLikedCheepList_ContainsUserLikedCheeps()
    {
        var email = "likeUser@test.com";
        var password = "Test123!";
        var username = "likeUser";

        await RegisterAccountIdentity(email, password, username);

        // Like an existing cheep by another user
        var cheep = await LikeCheep("Jacqualine Gilcoine");

        await _page.GotoAsync($"{ServerAddress}/info?page=1");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var likedCheep = _page.Locator("#messagelist > li").Filter(new() { HasText = "Jacqualine Gilcoine" }).First;
        await Expect(likedCheep).ToContainTextAsync("Jacqualine Gilcoine");
    }


    private async Task LoginAccountIdentity(string email, string password)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }


    private async Task RegisterAccountIdentity(string email, string password, string username)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(password);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
    }

    private async Task DeleteAccountIdentity()
    {
        await _page.GotoAsync(ServerAddress + "/info?page=1");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine($"URL: {_page.Url}");

        var forgetButton = _page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" });

        if (await forgetButton.CountAsync() == 0)
        {
            var bodyText = await _page.Locator("body").InnerTextAsync();
            Console.WriteLine($"Page content: {bodyText}");
            throw new Exception("'Forget me!' button not found on /info page");
        }

        await forgetButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task<ILocator> FollowUser(string username)
    {
        var cheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = username })
            .First;

        var followButton = cheep.GetByRole(AriaRole.Button, new() { Name = "Follow" });
        await followButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return cheep;
    }

    private async Task<ILocator> UnfollowUser(string username)
    {
        var cheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = username })
            .First;

        var unfollowButton = cheep.GetByRole(AriaRole.Button, new() { Name = "Unfollow" });
        await unfollowButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return cheep;
    }

    private async Task<ILocator> LikeCheep(string username)
    {
        var cheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = username })
            .First;

        await cheep.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var likeButton = cheep.GetByRole(AriaRole.Button, new() { Name = new("Like") }).First;
        await likeButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return cheep;
    }

    private async Task<ILocator> UnlikeCheep(string username)
    {
        var cheep = _page.Locator("#messagelist > li")
            .Filter(new() { HasText = username })
            .First;

        await cheep.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var unlikeButton = cheep.GetByRole(AriaRole.Button, new() { Name = new("Unlike") }).First;
        await unlikeButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return cheep;
    }
}