using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace net8cicd.Services
{
    public class TracingService
    {
        private static readonly ActivitySource ActivitySource = new("net8cicd-service");

        public void TraceSystemMetrics()
        {
            using var parent = ActivitySource.StartActivity("System Metrics Check", ActivityKind.Internal);
            if (parent == null) return;

            parent.SetTag("service.name", "net8cicd");
            parent.SetTag("environment", "development");

            // CPU Check
            using (var cpuSpan = ActivitySource.StartActivity(
                "CPU Usage Check",
                ActivityKind.Internal,
                parent.Context))
            {
                var cpuUsage = GetCpuUsage();
                cpuSpan?.SetTag("cpu.usage_percent", cpuUsage);
                if (cpuUsage > 80)
                {
                    cpuSpan?.SetStatus(ActivityStatusCode.Error, "High CPU usage detected");
                }
            }

            // Memory Check
            using (var memSpan = ActivitySource.StartActivity(
                "Memory Check",
                ActivityKind.Internal,
                parent.Context))
            {
                var memoryInfo = GetMemoryInfo();
                memSpan?.SetTag("memory.used_gb", memoryInfo.used);
                memSpan?.SetTag("memory.total_gb", memoryInfo.total);
            }

            // API Health Check
            using (var healthSpan = ActivitySource.StartActivity(
                "API Health Check",
                ActivityKind.Internal,
                parent.Context))
            {
                healthSpan?.SetTag("endpoint", "/health");
                healthSpan?.SetTag("status", "healthy");
            }

            parent?.SetStatus(ActivityStatusCode.Ok);

            using (var alarmSpan = ActivitySource.StartActivity("Alarm Evaluation", ActivityKind.Internal, parent!.Context))
            {
                var activeAlarms = CheckAlarms();
                alarmSpan?.SetTag("alarm.count", activeAlarms.Count);

                foreach (var alarm in activeAlarms)
                {
                    alarmSpan?.AddEvent(new ActivityEvent("alarm.raised", tags: new ActivityTagsCollection
        {
            { "alarm.name", alarm.Name },
            { "alarm.level", alarm.Level },
            { "alarm.message", alarm.Message }
        }));
                }

                if (activeAlarms.Any())
                {
                    alarmSpan?.SetStatus(ActivityStatusCode.Error, "Alarms detected during check.");
                }
            }

        }

        private List<(string Name, string Level, string Message)> CheckAlarms()
        {
            // Placeholder — you can wire this to your actual alarm logic
            return new List<(string, string, string)>
    {
        ("Low Disk Space", "Warning", "Only 5% disk space left on D:"),
        ("POS Offline", "Critical", "POS terminal #3 is unreachable.")
    };
        }


        private static double GetCpuUsage() => Random.Shared.Next(0, 100);

        private static (double used, double total) GetMemoryInfo() => (Random.Shared.Next(1, 8), 16);
    }
}
