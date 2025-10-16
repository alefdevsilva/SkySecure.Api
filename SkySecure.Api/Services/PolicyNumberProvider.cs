using SkySecure.Api.Services.Interfaces;

namespace SkySecure.Api.Services;


public class PolicyNumberProvider : IPolicyNumberProvider
{
    public string GeneratePolicyNumber()
    {
        return $"SKY{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

