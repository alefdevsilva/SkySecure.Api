using CSharpFunctionalExtensions;

namespace SkySecure.Api.Services.Interfaces;

public interface IPilotValidationService
{
    Task<Result> ValidateAsync(string pilotDocument);
}
