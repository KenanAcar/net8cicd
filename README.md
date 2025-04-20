# net8cicd
The repo contains the code for the .net 8 CICD demo that covers the following:
- OpennTelemetry activation via seperated config file

# opentelemetry packages
- dotnet add package OpenTelemetry.Extensions.Hosting
- dotnet add package OpenTelemetry.Instrumentation.AspNetCore
- dotnet add package OpenTelemetry.Instrumentation.Http
- dotnet add package OpenTelemetry.Exporter.Console
- dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol


# jeager all in one docker image
- jaegertracing/all-in-one:latest
