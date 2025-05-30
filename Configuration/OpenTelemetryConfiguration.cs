using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using System;

namespace net8cicd.Configuration;
public static class OpenTelemetryConfiguration
{
    public static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelSection = configuration.GetSection("OpenTelemetry");
        if(!otelSection.Exists()) return;

        var tracingEnabled = otelSection.GetValue<bool>("Tracing:Enabled");
        if (!tracingEnabled) return;

        var features = otelSection.GetSection("Tracing:Features");

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                if (features.GetValue<bool>("AspNetCore"))
                {
                    tracerProviderBuilder.AddAspNetCoreInstrumentation();
                }

                if (features.GetValue<bool>("HttpClient"))
                {
                    tracerProviderBuilder.AddHttpClientInstrumentation();
                }

                if (features.GetValue<bool>("ResourceAttributes"))
                {
                    tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(serviceName: configuration["Shop:Name"] ?? "UnknownShop")
                        .AddAttributes(new Dictionary<string, object>
                        {
                            ["shop.location"] = configuration["Shop:Location"] ?? "unknown",
                            ["shop.code"] = configuration["Shop:Code"] ?? "000"
                        })).AddSource("net8cicd-service");
                }

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
                                opt.Protocol = OtlpExportProtocol.Grpc; // 💡 IMPORTANT for SkyWalking
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
