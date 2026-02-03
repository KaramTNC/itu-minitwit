using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.None)]
[TestFixture]
public class InteractiveButtonTests : PageTest
{
    private string ServerAddress => GlobalTestSetup.ServerAddress;
    
    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(ServerAddress); // Always return to root homepage
    }

    [Test]
    public async Task BasicTest()
    { 
        var response = await Page.GotoAsync(ServerAddress);
        Assert.That(response!.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task PublicTimelineClickTest()
    {
        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Assert
        await Expect(Page.Locator("h2")).ToMatchAriaSnapshotAsync("- heading \"Public Timeline\" [level=2]");
    }

    [Test]
    public async Task LoginClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fixed: Added leading slash
        await Expect(Page).ToHaveURLAsync(ServerAddress + "/Identity/Account/Login");
    }

    [Test]
    public async Task RegisterClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Register" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fixed: Added leading slash
        await Expect(Page).ToHaveURLAsync(ServerAddress + "/Identity/Account/Register");
    }

    [Test]
    public async Task Timeline_CheepAuthorLinkTest()
    {
        // Navigate to the timeline page
        await Page.GotoAsync(ServerAddress);

        // Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        var cheepItems = Page.Locator("ul#messagelist li");
        int cheepCount = await cheepItems.CountAsync();

        if (cheepCount == 0)
        {
            Assert.Inconclusive("No cheeps exist on the timeline. Cannot test author links.");
        }
        else
        {
            for (int i = 0; i < cheepCount; i++)
            {
                var cheepItem = cheepItems.Nth(i);
                var link = cheepItem.Locator("strong a");

                // Wait for link to appear
                await link.WaitForAsync(new() { Timeout = 5000 });

                // Assert link is visible and has text
                Assert.IsTrue(await link.IsVisibleAsync(), $"Cheep #{i + 1}: Author link is not visible.");
                var authorName = await link.InnerTextAsync();
                Assert.IsFalse(string.IsNullOrWhiteSpace(authorName), $"Cheep #{i + 1}: Author link text is empty.");

                // Click the link and verify the timeline heading
                await link.ClickAsync();
                await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");

                // Optionally, navigate back to the main timeline for the next iteration
                await Page.GoBackAsync();
            }
        }
    }
}