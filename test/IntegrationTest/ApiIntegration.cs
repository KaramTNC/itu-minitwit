using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SupportScripts;
using Xunit.Extensions.Ordering;

namespace test;

[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<ApiCustom> { }

[Collection("Api")]
[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class ApiIntegration
{
    private readonly ApiCustom _factory;

    protected const string Username = "simulator";
    protected const string Password = "super_safe!";

    public ApiIntegration(ApiCustom factory)
    {
        _factory = factory;
    }

    [Fact, Order(1)]
    public async Task TestLatest()
    {
        // Post something to update LATEST
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var data = new
        {
            username = "test",
            email = "test@test",
            pwd = "Test123!",
        };
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/register?latest=1337", content);
        Assert.True(response.IsSuccessStatusCode);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(1337, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(2)]
    public async Task TestRegister()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        // Register a new user
        var data = new
        {
            username = "a",
            email = "a@a.a",
            pwd = "a",
        };
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/register?latest=1", content);
        Assert.True(response.IsSuccessStatusCode);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(1, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(3)]
    public async Task TestCreateMsg()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        // Post a message for user 'a'
        var username = "a";
        var data = new { content = "Blub!" };
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync($"/msgs/{username}?latest=2", content);
        Assert.True(response.IsSuccessStatusCode);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(2, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(4)]
    public async Task TestGetLatestUserMsgs()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        // Get messages for user 'a'
        var username = "a";
        var response = await client.GetAsync($"/msgs/{username}?no=20&latest=3");
        Assert.Equal(200, (int)response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<JsonElement[]>(body);

        var gotItEarlier = false;
        foreach (var msg in messages!)
        {
            if (
                msg.GetProperty("content").GetString() == "Blub!"
                && msg.GetProperty("user").GetString() == username
            )
            {
                gotItEarlier = true;
            }
        }
        Assert.True(gotItEarlier);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(3, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(5)]
    public async Task TestGetLatestMsgs()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        // Get all latest messages
        var username = "a";
        var response = await client.GetAsync("/msgs?no=20&latest=4");
        Assert.Equal(200, (int)response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<JsonElement[]>(body);

        var gotItEarlier = false;
        foreach (var msg in messages!)
        {
            if (
                msg.GetProperty("content").GetString() == "Blub!"
                && msg.GetProperty("user").GetString() == username
            )
            {
                gotItEarlier = true;
            }
        }
        Assert.True(gotItEarlier);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(4, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(6)]
    public async Task TestRegisterB()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        // Register user 'b'
        var data = new
        {
            username = "b",
            email = "b@b.b",
            pwd = "b",
        };
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/register?latest=5", content);
        Assert.True(response.IsSuccessStatusCode);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(5, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(7)]
    public async Task TestRegisterC()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var data = new
        {
            username = "c",
            email = "c@c.c",
            pwd = "c",
        };
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/register?latest=6", content);
        Assert.True(response.IsSuccessStatusCode);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(6, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(8)]
    public async Task TestFollowUser()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var username = "a";
        var url = $"/fllws/{username}";

        // Follow 'b'
        var followB = new StringContent(
            JsonSerializer.Serialize(new { follow = "b" }),
            Encoding.UTF8,
            "application/json"
        );
        var response = await client.PostAsync($"{url}?latest=7", followB);
        Assert.True(response.IsSuccessStatusCode);

        // Follow 'c'
        var followC = new StringContent(
            JsonSerializer.Serialize(new { follow = "c" }),
            Encoding.UTF8,
            "application/json"
        );
        response = await client.PostAsync($"{url}?latest=8", followC);
        Assert.True(response.IsSuccessStatusCode);

        // Get follows list
        response = await client.GetAsync($"{url}?no=20&latest=9");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        var follows = json.GetProperty("follows")
            .EnumerateArray()
            .Select(f => f.GetString())
            .ToList();
        Assert.Contains("b", follows);
        Assert.Contains("c", follows);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        body = await response.Content.ReadAsStringAsync();
        json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(9, json.GetProperty("latest").GetInt32());
    }

    [Fact, Order(9)]
    public async Task TestAUnfollowsB()
    {
        var client = _factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            credentials
        );

        var username = "a";
        var url = $"/fllws/{username}";

        // Unfollow 'b'
        var unfollowB = new StringContent(
            JsonSerializer.Serialize(new { unfollow = "b" }),
            Encoding.UTF8,
            "application/json"
        );
        var response = await client.PostAsync($"{url}?latest=10", unfollowB);
        Assert.True(response.IsSuccessStatusCode);

        // Verify 'b' is no longer in follows list
        response = await client.GetAsync($"{url}?no=20&latest=11");
        Assert.True(response.IsSuccessStatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        var follows = json.GetProperty("follows")
            .EnumerateArray()
            .Select(f => f.GetString())
            .ToList();
        Assert.DoesNotContain("b", follows);

        // Verify that latest was updated
        response = await client.GetAsync("/latest");
        body = await response.Content.ReadAsStringAsync();
        json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(11, json.GetProperty("latest").GetInt32());
    }
}
