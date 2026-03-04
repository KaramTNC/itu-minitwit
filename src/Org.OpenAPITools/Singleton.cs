using System.Diagnostics.Metrics;

public sealed class Singleton
{
    private static readonly Singleton instance = new Singleton();

    public int latest = 0;
    static Meter s_meter = new("API", "1.0.0");
    static Counter<int> s_latestRequestCounter = s_meter.CreateCounter<int>(
        name: "latest_request_count",
        description: "Number of requests to the /latest endpoint",
        unit: "requests");

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static Singleton()
    {
    }

    private Singleton()
    {

    }

    public static Singleton Instance
    {
        get
        {
            return instance;
        }
    }

    public void IncrementLatestCounter()
    {
        s_latestRequestCounter.Add(1);
    }
}