namespace Sheffield.Client.WebApi;

using AspNetCore.SignalR.OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;

public sealed class Program
{
    private Program()
    {
    }

    public static async Task<int> Main(string[] args)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddContainerDetector()
                .AddHostDetector()
                .AddOperatingSystemDetector()
                .AddProcessDetector()
                .AddProcessRuntimeDetector()
            ;

        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.AddOtlpExporter();
        });

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/healthz");
                })
                .AddHttpClientInstrumentation(options => options.RecordException = true)
                .AddSignalRInstrumentation()
                .AddSource(
                    "Microsoft.Orleans.Application",
                    "Microsoft.Orleans.Runtime",
                    "Microsoft.Orleans.Lifecycle",
                    "Microsoft.Orleans.Storage",
                    "Microsoft.Orleans.DurableJobs")
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("Microsoft.Orleans")
                .AddProcessInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter());

        builder.Services.AddHealthChecks();

        var consulAclToken = builder.Configuration["Consul:AclToken"];

        if (string.IsNullOrWhiteSpace(consulAclToken))
        {
            throw new InvalidOperationException("Consul:AclToken is not configured.");
        }

        builder.Host.UseOrleansClient(client =>
        {
            client.AddActivityPropagation();

            client.UseConsulClientClustering(options =>
            {
                options.ConfigureConsulClient(
                    new Uri(builder.Configuration["Consul:Address"]!),
                    consulAclToken);
            });

            client.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = builder.Configuration["Orleans:ClusterId"]!;
                options.ServiceId = builder.Configuration["Orleans:ServiceId"]!;
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.UseHealthChecks("/healthz");

        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Client API v1");
            
        });

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();

        return Environment.ExitCode;
    }
}
