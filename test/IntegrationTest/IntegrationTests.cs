using SupportScripts;

namespace test;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task GetFromSpecificAuthor()
    {
        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("/Helge");
        response.EnsureSuccessStatusCode();
        var readResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("Helge", readResponse);
    }

    [Fact]
    public async Task GetAll()
    {
        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("");
        response.EnsureSuccessStatusCode();
        var readResponse = await response.Content.ReadAsStringAsync();
        
        Assert.Contains("The train pulled up at his bereavement; but his eyes riveted upon that heart for ever; who ever conquered it?", readResponse);
    }
}