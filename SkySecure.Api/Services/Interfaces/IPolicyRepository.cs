namespace SkySecure.Api.Services.Interfaces;

public interface IPolicyRepository
{
    Task SavePolicyAsync(Models.PolicyRequest request, string policyNumber, decimal premium);
}
