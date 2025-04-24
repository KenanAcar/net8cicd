using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace net8cicd.Logging
{
public static class NLogConfiguration
{
    public static void ConfigureNLog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseNLog(); // Automatically loads "nlog.config"
    }
}
}
