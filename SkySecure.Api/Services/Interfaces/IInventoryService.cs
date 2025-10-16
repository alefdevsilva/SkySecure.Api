namespace SkySecure.Api.Services.Interfaces;

public interface IInventoryService
{
    Task IncrementPolicyCountAsync(string droneModel);
}