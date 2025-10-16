namespace SkySecure.Functions.Services.Interfaces;

public interface IInventoryService
{
    Task IncrementPolicyCountAsync(string droneModel);
}