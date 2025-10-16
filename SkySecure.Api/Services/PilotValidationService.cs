using CSharpFunctionalExtensions;
using SkySecure.Api.Models;
using SkySecure.Api.Services.Interfaces;
using System.Text;
using System.Text.Json;

public class PilotValidationService : IPilotValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PilotValidationService> _logger;
    private readonly IConfiguration _config;

    public PilotValidationService(IHttpClientFactory httpClientFactory, ILogger<PilotValidationService> logger, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }

    public async Task<Result> ValidateAsync(string pilotDocument)
    {
        try
        {
            var url = _config["Anac:ValidatePilotUrl"] ?? throw new InvalidOperationException("ANAC URL not configured");
            var client = _httpClientFactory.CreateClient("external");
            var payload = new { document = pilotDocument };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await client.PostAsync(url, content).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("ANAC returned {Status} for document {Doc}", resp.StatusCode, pilotDocument);
                return Result.Failure($"ANAC returned {resp.StatusCode} for document {pilotDocument}");
            }

            var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<AnacResponse>(body);

            if (result is null || result.IsValid)
                return Result.Failure("Request Failed");

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ANAC validation failed for {Doc}", pilotDocument);
            return Result.Failure($"ANAC validation failed for {pilotDocument}");
        }
    }
}
