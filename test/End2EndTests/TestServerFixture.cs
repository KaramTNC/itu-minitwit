using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Web;

namespace PlaywrightTests;

public class TestServerFixture : IAsyncDisposable
{
    private static bool _serverStarted;
    private static WebApplication? _app;
    private static SqliteConnection? _keepAliveConnection;
    public string ServerAddress = "http://localhost:5273";

    public async Task StartAsync()
    {
        
        if (!_serverStarted)
        {
            // Check if port is already in use
            try
            {
                using var testClient = new HttpClient();
                testClient.Timeout = TimeSpan.FromSeconds(1);
                var testResult = await testClient.GetAsync(ServerAddress);
                Console.WriteLine($"WARNING: Port 5273 is already in use! Got status: {testResult.StatusCode}");
            }
            catch
            {
                Console.WriteLine("Port 5273 is free, proceeding with server start");
            }

            // Create and keep open a connection to prevent in-memory DB from being destroyed
            _keepAliveConnection = new SqliteConnection("DataSource=TestDb;Mode=Memory;Cache=Shared");
            _keepAliveConnection.Open();
            Console.WriteLine("Opened persistent in-memory database connection");

            _app = Program.BuildWebApplication(environment: "Testing");

            // Initialize the in-memory database
            using (var scope = _app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
                context.Database.EnsureCreated();
                DbInitializer.SeedDatabase(context);
                Console.WriteLine("Database initialized and seeded");
            }

            Console.WriteLine("Routes:");
            foreach (var d in _app.Services.GetRequiredService<EndpointDataSource>().Endpoints)
            {
                Console.WriteLine(d.DisplayName);
            }

            Console.WriteLine("Content root: " + _app.Environment.ContentRootPath);
            Console.WriteLine("Pages folder exists? " +
                              Directory.Exists(Path.Combine(_app.Environment.ContentRootPath, "Pages")));
            Console.WriteLine("wwwroot folder exists? " +
                              Directory.Exists(Path.Combine(_app.Environment.ContentRootPath, "wwwroot")));

            _app.Urls.Clear();
            _app.Urls.Add(ServerAddress);

            try
            {
                // Start the server in a background task
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _app.RunAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Server crashed: " + ex.Message);
                    }
                });

                Console.WriteLine("Server starting on " + ServerAddress);

                // Give it a moment to actually start listening
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER FAILED TO START:");
                Console.WriteLine(ex);
                throw;
            }

            _serverStarted = true;

            // Wait until server is actually reachable with retries
            bool serverReady = false;
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            for (int i = 0; i < 30; i++)
            {
                try
                {
                    Console.WriteLine($"Attempt {i + 1}: Trying to connect to {ServerAddress}");
                    var result = await httpClient.GetAsync(ServerAddress);
                    Console.WriteLine($"Attempt {i + 1}: Got status code {result.StatusCode}");

                    if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 600)
                    {
                        serverReady = true;
                        Console.WriteLine($"Server is ready and responding with status {result.StatusCode}");
                        break;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: HttpRequestException - {ex.Message}");
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: Timeout - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: {ex.GetType().Name} - {ex.Message}");
                }

                await Task.Delay(500);
            }

            if (!serverReady)
            {
                throw new Exception(
                    $"Server failed to become ready within timeout period. Check if port 5273 is available and not blocked by firewall.");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }

        if (_keepAliveConnection != null)
        {
            _keepAliveConnection.Close();
            _keepAliveConnection.Dispose();
            Console.WriteLine("Closed in-memory database connection");
        }
    }
}