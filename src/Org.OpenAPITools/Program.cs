using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Org.OpenAPITools
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        public Singleton single = Singleton.Instance;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var metricsUriPrefix =
                Environment.GetEnvironmentVariable("OTEL_PROMETHEUS_URI_PREFIX")
                ?? "http://*:9185/";

            // Necessary for monitoring metrics
            // Initialises a metrics endpoint where Prometheus can scrape (and store) the metrics gathered by OpenTelemetry.
            using MeterProvider meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddMeter("API") // This meter is currently the only one used. We'll need to add more meters using .AddMeter later on.
                .AddView(
                    instrumentName: "*request_time",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[]
                        {
                            0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2, 5, 10, 15, 20
                        }
                })
                .AddPrometheusHttpListener(options =>
                    options.UriPrefixes = new string[] { metricsUriPrefix }
                ) // endpoint is http://<host>:9185/metrics
                .Build();

            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create the host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:8080/");
                });
    }
}
