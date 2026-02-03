
using Microsoft.Playwright;

namespace PlaywrightTests;

/// <summary>
/// Global setup that runs once for the entire test assembly.
/// This ensures the test server starts once and stays alive for all test classes.
/// </summary>
[SetUpFixture]
public class GlobalTestSetup
{
    private static TestServerFixture? _fixture;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    
    public static string ServerAddress => _fixture?.ServerAddress ?? 
                                          throw new InvalidOperationException("Test server not started");
    
    public static IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not started");


    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        Console.WriteLine("=== GLOBAL SETUP: Starting test server ===");
        _fixture = new TestServerFixture();
        await _fixture.StartAsync();
        Console.WriteLine($"=== GLOBAL SETUP: Test server started at {ServerAddress} ===");
        
        
        // Start Playwright and launch browser in headed mode
        _playwright = await Playwright.CreateAsync();
        var isCiCd = Environment.GetEnvironmentVariable("CI") == "true";

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = isCiCd,      // headed locally, headless in CI
            SlowMo = isCiCd ? 0 : 100
        });

        
        Console.WriteLine("=== GLOBAL SETUP: Browser launched in headed mode ===");
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        Console.WriteLine("=== GLOBAL TEARDOWN: Stopping test server ===");
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
            _fixture = null;
        }
        
        Console.WriteLine("=== GLOBAL TEARDOWN: Closing browser ===");
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        if (_playwright != null)
        {
            _playwright.Dispose();
            _playwright = null;
        }

        Console.WriteLine("=== GLOBAL TEARDOWN: Test server and browser stopped ===");
    }
}