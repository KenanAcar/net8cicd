# net8cicd
The repo contains the code for the .net 8 CICD demo that covers the following:
- OpenTelemetry activation via seperated config file
- NLog to seq
# opentelemetry packages
- dotnet add package OpenTelemetry.Extensions.Hosting
- dotnet add package OpenTelemetry.Instrumentation.AspNetCore
- dotnet add package OpenTelemetry.Instrumentation.Http
- dotnet add package OpenTelemetry.Exporter.Console
- dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol

# nlog packages
- dotnet add package NLog.Web.AspNetCore
- dotnet add package NLog.Targets.Seq


# docker images
- jaegertracing/all-in-one:latest
- datalust/seq

# summary

| Goal | Tool | How |
| --- | --- |--- |
| Structured logging to UI | NLog ➜ Seq | NLog.Targets.Seq
| Distributed tracing across layers | OTel ➜ Jaeger | OTel SDK + Jaeger exporter
| View logs | Seq | http://localhost:5341
| View traces | Jaeger | http://localhost:16686
| Correlate logs + traces | Manual TraceId | Add TraceId from Activity to logs

## Traces → Created automatically by:

    AddAspNetCoreInstrumentation() – incoming HTTP

    AddHttpClientInstrumentation() – outgoing HTTP

    AddSqlClientInstrumentation() – DB calls

    ActivitySource.StartActivity(...) – optional fine-grained spans

## Logs → Created manually at:
| Example Use Cases | Example Log Statement |
| --- | --- |
| ⚠️ Validation failed | logger.Warn("Validation failed for user {UserId}", user.Id); |
| 🛑 Business exception occurred | logger.Error(ex, "Payment failed for order {OrderId}", order.Id); |
| 📈 Important business events | logger.Info("User {UserId} completed checkout", user.Id); |
| ✅ Audit / compliance record | logger.Info("Admin {AdminId} updated settings at {Time}", ...) |
| 🔍 Debugging | logger.Debug("User {UserId} accessed resource {ResourceId}", ...) |

## sonarqube

## docker
```
docker run -d --name sonarqube -p 9000:9000 -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true sonarqube:lts
```

## manual enabling
Scanner .NET Core Global Tool

As a prerequisite you need to have the sonarscanner tool installed globally using the following command:

```
dotnet tool install --global dotnet-sonarscanner
```
Make sure dotnet tools folder is in your path. See dotnet global tools documentation for more information.
Execute the Scanner

Running a SonarQube analysis is straighforward. You just need to execute the following commands at the root of your solution.
```
dotnet sonarscanner begin /k:"dotnet8cicd" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="sqp_0c0fb5b08a9f69e3c21653b13583b91a896d1825"
```
```
dotnet build
```

```
dotnet sonarscanner end /d:sonar.login="sqp_0c0fb5b08a9f69e3c21653b13583b91a896d1825"

```