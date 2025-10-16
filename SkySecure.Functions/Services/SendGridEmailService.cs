using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkySecure.Functions.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SkySecure.Functions.Services;

public class SendGridEmailService : IEmailService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly IConfiguration _config;

    public SendGridEmailService(IHttpClientFactory factory, ILogger<SendGridEmailService> logger, IConfiguration config)
    {
        _factory = factory;
        _logger = logger;
        _config = config;
    }

    public async Task SendPolicyEmailAsync(string to, string pdfUrl, string policyNumber)
    {
        try
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var endpoint = _config["SendGrid:Endpoint"];
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                _logger.LogWarning("SendGrid not configured");
                return;
            }

            var client = _factory.CreateClient("external");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                personalizations = new[] {
                    new { to = new[] { new { email = to } }, subject = $"Your drone policy {policyNumber}" }
                },
                from = new { email = "noreply@skysecure.com" },
                content = new[] { new { type = "text/plain", value = "Your policy is attached." } },
                attachments = new[] {
                    new {
                        content = Convert.ToBase64String(await File.ReadAllBytesAsync(pdfUrl)),
                        filename = Path.GetFileName(pdfUrl),
                        type = "application/pdf",
                        disposition = "attachment"
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync(endpoint, content);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to send email: {Status}", resp.StatusCode);
            }
            else
            {
                _logger.LogInformation("Email sent to {To} for policy {Policy}", to, policyNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
        }
    }
}
