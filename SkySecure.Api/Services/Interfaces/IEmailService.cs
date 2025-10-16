namespace SkySecure.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendPolicyEmailAsync(string to, string pdfUrl, string policyNumber);
}
