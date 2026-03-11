using System.Collections.Generic;
using System.Diagnostics.Metrics;

public sealed class Singleton
{
    private static readonly Singleton instance = new Singleton();

    public int latest = 0;
    static Meter s_meter = new("API", "1.0.0");

    static Counter<int> s_getFollowersRequestCounter = s_meter.CreateCounter<int>(
        name: "get_followers_request_count",
        description: "Number of GET requests to the /fllws endpoint",
        unit: "requests");

    static Counter<int> s_postFollowersRequestCounter = s_meter.CreateCounter<int>(
        name: "post_followers_request_count",
        description: "Number of POST requests to the /fllws endpoint",
        unit: "requests");

    static Counter<int> s_getLatestRequestCounter = s_meter.CreateCounter<int>(
        name: "get_latest_request_count",
        description: "Number of requests to the /latest endpoint",
        unit: "requests");

    static Counter<int> s_getMessagesRequestCounter = s_meter.CreateCounter<int>(
        name: "get_messages_request_count",
        description: "Number of GET requests to the /msgs endpoint",
        unit: "requests");

    static Counter<int> s_getMessagesPerUserCounter = s_meter.CreateCounter<int>(
        name: "get_messages_per_user_count",
        description: "Number of GET requests to the /msgs/{username} endpoint",
        unit: "requests");

    static Counter<int> s_postMessagesPerUserRequestCounter = s_meter.CreateCounter<int>(
        name: "post_messages_per_user_request_count",
        description: "Number of POST requests to the /msgs/{username} endpoint",
        unit: "requests");

    static Counter<int> s_postRegisterRequestCounter = s_meter.CreateCounter<int>(
        name: "post_register_request_count",
        description: "Number of POST requests to the /register endpoint",
        unit: "requests");

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static Singleton() { }

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

    public void IncrementLatestCounter(int statusCode)
    {
        s_getLatestRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementGetFollowersCounter(int statusCode)
    {
        s_getFollowersRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementPostFollowersCounter(int statusCode)
    {
        s_postFollowersRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementGetMessagesCounter(int statusCode)
    {
        s_getMessagesRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementGetMessagesPerUserCounter(int statusCode)
    {
        s_getMessagesPerUserCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementPostMessagesPerUserCounter(int statusCode)
    {
        s_postMessagesPerUserRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }

    public void IncrementPostRegisterCounter(int statusCode)
    {
        s_postRegisterRequestCounter.Add(1, new KeyValuePair<string, object?>("status_code", statusCode.ToString()));
    }
}