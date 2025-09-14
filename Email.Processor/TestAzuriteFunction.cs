using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using System.Text;

namespace Email.Processor;

public class TestAzuriteFunction
{
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;

    public TestAzuriteFunction(IConfiguration configuration, BlobServiceClient blobServiceClient)
    {
        _configuration = configuration;
        _blobServiceClient = blobServiceClient;
    }

    [OpenApiOperation(operationId: "TestAzurite_HttpStart", tags: new[] { "testing" }, Summary = "Starts the Azurite test orchestration", Description = "This starts a durable orchestration that creates a container, uploads a blob, and lists blobs in Azurite.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Accepted, contentType: "application/json", bodyType: typeof(string), Description = "The orchestration status response.")]
    [Function("TestAzurite_HttpStart")]
    public async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("TestAzurite_HttpStart");

        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(TestAzuriteOrchestrator));

        logger.LogInformation("Started orchestration with ID = {instanceId}", instanceId);

        return client.CreateCheckStatusResponse(req, instanceId);
    }

    [Function(nameof(TestAzuriteOrchestrator))]
    public async Task<string> TestAzuriteOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // Ejecutar las actividades en secuencia
        var result1 = await context.CallActivityAsync<string>(nameof(CreateContainerActivity), null);
        var result2 = await context.CallActivityAsync<string>(nameof(UploadBlobActivity), null);
        var result3 = await context.CallActivityAsync<string>(nameof(ListBlobsActivity), null);

        return $"{result1}\n{result2}\n{result3}";
    }

    [Function(nameof(CreateContainerActivity))]
    public async Task<string> CreateContainerActivity([ActivityTrigger] object input, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("CreateContainerActivity");

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("test-container");

            await containerClient.CreateIfNotExistsAsync();
            logger.LogInformation("Contenedor creado exitosamente");

            return "✅ Contenedor 'test-container' creado en Azurite";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creando contenedor");
            return $"❌ Error creando contenedor: {ex.Message}";
        }
    }

    [Function(nameof(UploadBlobActivity))]
    public async Task<string> UploadBlobActivity([ActivityTrigger] object input, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("UploadBlobActivity");

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("test-container");
            var blobClient = containerClient.GetBlobClient("test-blob.txt");

            var content = $"Test desde Durable Function - {DateTime.UtcNow}";
            var contentBytes = Encoding.UTF8.GetBytes(content);

            using var stream = new MemoryStream(contentBytes);
            await blobClient.UploadAsync(stream, overwrite: true);

            logger.LogInformation("Blob subido exitosamente");
            return "✅ Blob 'test-blob.txt' subido a Azurite";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error subiendo blob");
            return $"❌ Error subiendo blob: {ex.Message}";
        }
    }

    [Function(nameof(ListBlobsActivity))]
    public async Task<string> ListBlobsActivity([ActivityTrigger] object input, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("ListBlobsActivity");

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("test-container");

            var blobs = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }

            logger.LogInformation("Blobs listados exitosamente: {Count} blobs", blobs.Count);
            return $"✅ Blobs en contenedor: {string.Join(", ", blobs)}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listando blobs");
            return $"❌ Error listando blobs: {ex.Message}";
        }
    }
}