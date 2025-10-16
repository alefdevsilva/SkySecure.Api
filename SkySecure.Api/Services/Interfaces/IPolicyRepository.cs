using CSharpFunctionalExtensions;

namespace SkySecure.Api.Services.Interfaces;

public interface IPolicyRepository
{
    Task<Maybe<bool>> SavePolicyAsync(Models.PolicyRequest request, string policyNumber, decimal premium);
}
