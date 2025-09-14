using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;

namespace Email.Processor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();

                    // Register BlobServiceClient for dependency injection
                    services.AddSingleton(x => 
                    {
                        return new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                    });
                })
                .Build();

            host.Run();
        }
    }
}
