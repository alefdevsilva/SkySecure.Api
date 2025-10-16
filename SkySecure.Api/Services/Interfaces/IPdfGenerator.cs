namespace SkySecure.Api.Services.Interfaces;

public interface IPdfGenerator
{
    Task<string> GeneratePdfAsync(Models.PolicyRequest request, string policyNumber, decimal premium);
}