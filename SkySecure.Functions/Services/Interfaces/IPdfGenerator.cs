using SkySecure.Functions.Models;

namespace SkySecure.Functions.Services.Interfaces;

public interface IPdfGenerator
{
    Task<string> GeneratePdfAsync(PolicyRequest request, string policyNumber, decimal premium);
}