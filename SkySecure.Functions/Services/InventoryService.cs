using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkySecure.Functions.Services.Interfaces;

namespace SkySecure.Functions.Services;

public class InventoryService : IInventoryService
{
    private readonly IConfiguration _config;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IConfiguration config, ILogger<InventoryService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task IncrementPolicyCountAsync(string droneModel)
    {
        try
        {
            var cs = _config.GetConnectionString("DefaultConnection");
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            var q = "UPDATE DroneInventory SET PoliciesIssued = PoliciesIssued + 1 WHERE [DroneModel] = @Model";
            await using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.Add(new SqlParameter("@Model", droneModel));
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update inventory for model {Model}", droneModel);
            throw;
        }
    }
}