using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.Metrics;

public sealed class Singleton
{
    private const string REQUESTS_UNIT = "requests";
    private static readonly Singleton instance = new Singleton();

    public int latest = 0;
    static Meter s_meter = new("API", "1.0.0");

    //Histogram for request times
    static Histogram<double> PostFollowhistogram = s_meter.CreateHistogram<double>(
        name: "PostFollow_request_time",
        unit: "ms",
        description: "The time taken to handle an http request for PostFollow"
    );
    static Histogram<double> LatestHistogram = s_meter.CreateHistogram<double>(
        name: "GetLatest_request_time",
        unit: "ms",
        description: "The time taken to handle an http request for GetLatest"
    );
    static Histogram<double> MsgsHistogram = s_meter.CreateHistogram<double>(
        name: "PostMsgs_request_time",
        unit: "ms",
        description: "The time taken to handle an http request for PostMsgs"
    );
    static Histogram<double> RegisterHistogram = s_meter.CreateHistogram<double>(
        name: "PostRegister_request_time",
        unit: "ms",
        description: "The time taken to handle an http request for PostRegister"
    );

    //Counters for requests
    static Counter<int> s_getFollowersRequestCounter = s_meter.CreateCounter<int>(
        name: "get_followers",
        description: "Number of GET requests to the /fllws endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_postFollowersRequestCounter = s_meter.CreateCounter<int>(
        name: "post_followers",
        description: "Number of POST requests to the /fllws endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_getLatestRequestCounter = s_meter.CreateCounter<int>(
        name: "get_latest",
        description: "Number of requests to the /latest endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_getMessagesRequestCounter = s_meter.CreateCounter<int>(
        name: "get_messages",
        description: "Number of GET requests to the /msgs endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_getMessagesPerUserRequestCounter = s_meter.CreateCounter<int>(
        name: "get_messages_per_user",
        description: "Number of GET requests to the /msgs/{username} endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_postMessagesPerUserRequestCounter = s_meter.CreateCounter<int>(
        name: "post_messages_per_user",
        description: "Number of POST requests to the /msgs/{username} endpoint",
        unit: REQUESTS_UNIT
    );

    static Counter<int> s_postRegisterRequestCounter = s_meter.CreateCounter<int>(
        name: "post_register",
        description: "Number of POST requests to the /register endpoint",
        unit: REQUESTS_UNIT
    );

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static Singleton() { }

    private Singleton() { }

    public static Singleton Instance
    {
        get { return instance; }
    }

    //Functions to call to increment request counters
    public void IncrementLatestCounter(int statusCode)
    {
        s_getLatestRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementGetFollowersCounter(int statusCode)
    {
        s_getFollowersRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementPostFollowersCounter(int statusCode)
    {
        s_postFollowersRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementGetMessagesCounter(int statusCode)
    {
        s_getMessagesRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementGetMessagesPerUserCounter(int statusCode)
    {
        s_getMessagesPerUserRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementPostMessagesPerUserCounter(int statusCode)
    {
        s_postMessagesPerUserRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    public void IncrementPostRegisterCounter(int statusCode)
    {
        s_postRegisterRequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("status_code", statusCode.ToString())
        );
    }

    //Functions to call to add to request histograms
    public void PostFollowHistogram(Stopwatch sw)
    {
        PostFollowhistogram.Record(sw.Elapsed.TotalMilliseconds);
    }

    public void GetLatestHistogram(Stopwatch sw)
    {
        LatestHistogram.Record(sw.Elapsed.TotalMilliseconds);
    }

    public void PostMsgsHistogram(Stopwatch sw)
    {
        MsgsHistogram.Record(sw.Elapsed.TotalMilliseconds);
    }

    public void PostRegisterHistogram(Stopwatch sw)
    {
        RegisterHistogram.Record(sw.Elapsed.TotalMilliseconds);
    }
}
