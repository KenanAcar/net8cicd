using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using System;

public static class OpenTelemetryConfiguration
{
    public static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelSection = configuration.GetSection("OpenTelemetry");
        var tracingEnabled = otelSection.GetValue<bool>("Tracing:Enabled");

        if (!tracingEnabled) return;

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
    .AddService(serviceName: configuration["Shop:Name"] ?? "UnknownShop")
    .AddAttributes(new Dictionary<string, object>
    {
        ["shop.location"] = configuration["Shop:Location"] ?? "unknown",
        ["shop.code"] = configuration["Shop:Code"] ?? "000"
    }));


                var exporter = otelSection.GetValue<string>("Tracing:Exporter")?.ToLowerInvariant();

                switch (exporter)
                {
                    case "console":
                        tracerProviderBuilder.AddConsoleExporter();
                        break;

                    case "otlp":
                        var endpoint = otelSection.GetValue<string>("Tracing:Otlp:Endpoint");
                        if (!string.IsNullOrWhiteSpace(endpoint))
                        {
                            tracerProviderBuilder.AddOtlpExporter(opt =>
                            {
                                opt.Endpoint = new Uri(endpoint);
                            });
                        }
                        else
                        {
                            throw new InvalidOperationException("OTLP endpoint must be configured when using OTLP exporter.");
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported exporter: {exporter}");
                }
            });
    }
}
