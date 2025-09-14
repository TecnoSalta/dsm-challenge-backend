using Microsoft.AspNetCore.Mvc;

namespace PWC.Challenge.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TestController> _logger;

    public TestController(IHttpClientFactory httpClientFactory, ILogger<TestController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("trigger-azurite-test")]
    public async Task<IActionResult> TriggerAzuriteTest()
    {
        try
        {
            _logger.LogInformation("Iniciando test de Azurite via Durable Function");

            var client = _httpClientFactory.CreateClient();

            // Llamar a la Durable Function dentro de Docker
            var response = await client.PostAsync(
                "http://pwc.challenge.email.processor:7071/api/TestAzurite_HttpStart",
                null);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Durable Function triggered successfully: {Content}", content);

                return Ok(new
                {
                    success = true,
                    message = "Test de Azurite iniciado",
                    statusUrl = response.Headers.GetValues("Location").FirstOrDefault()
                });
            }

            _logger.LogError("Error triggering function: {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error triggering function");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en test de Azurite");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("test-simple")]
    public async Task<IActionResult> TestSimple()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://pwc.challenge.email.processor:7071/api/TestAzurite_HttpStart");

            return response.IsSuccessStatusCode
                ? Ok("✅ Durable Function respondió correctamente")
                : StatusCode((int)response.StatusCode, "❌ Durable Function no respondió");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}