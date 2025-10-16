using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkySecure.Functions.Models;
using SkySecure.Functions.Services.Interfaces;

namespace SkySecure.Functions.Services;

public class LocalPdfGenerator : IPdfGenerator
{
    private readonly ILogger<LocalPdfGenerator> _logger;
    private readonly IConfiguration _config;

    public LocalPdfGenerator(ILogger<LocalPdfGenerator> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<string> GeneratePdfAsync(PolicyRequest request, string policyNumber, decimal premium)
    {
        var baseFolder = _config["Storage:LocalFolder"] ?? Path.GetTempPath();
        var safeClient = string.IsNullOrWhiteSpace(request.ClientName) ? "unknown" : SanitizeFileName(request.ClientName);
        var folder = Path.Combine(baseFolder, safeClient);
        Directory.CreateDirectory(folder);

        var filename = $"policy_{policyNumber}.pdf";
        var filePath = Path.Combine(folder, filename);

        await File.WriteAllTextAsync(filePath, $"Policy: {policyNumber}\nPremium: {premium:C}\nClient: {request.ClientName}");
        _logger.LogInformation("PDF generated at {Path}", filePath);

        return filePath;
    }

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '-');
        return name;
    }
}
