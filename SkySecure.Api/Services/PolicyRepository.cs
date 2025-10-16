using Dapper;
using Microsoft.Data.SqlClient;
using SkySecure.Api.Services.Interfaces;

namespace SkySecure.Api.Services
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PolicyRepository> _logger;

        public PolicyRepository(IConfiguration config, ILogger<PolicyRepository> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SavePolicyAsync(Models.PolicyRequest request, string policyNumber, decimal premium)
        {
            try
            {
                var cs = _config.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string missing");

                await using var conn = new SqlConnection(cs);

                var query = @"INSERT INTO Policies (PolicyNumber, ClientName, ClientEmail, DroneModel, DroneValue, Premium, PilotDocument, OperationState, CreatedAt, Status)
                      VALUES (@PolicyNumber, @ClientName, @ClientEmail, @DroneModel, @DroneValue, @Premium, @PilotDocument, @OperationState, @CreatedAt, @Status)";

                await conn.ExecuteAsync(query, new
                {
                    PolicyNumber = policyNumber,
                    ClientName = request.ClientName,
                    ClientEmail = request.ClientEmail,
                    DroneModel = request.DroneModel,
                    DroneValue = request.DroneValue,
                    Premium = premium,
                    PilotDocument = request.PilotDocument,
                    OperationState = request.OperationState,
                    CreatedAt = DateTime.UtcNow,
                    Status = "ACTIVE"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving policy {PolicyNumber}", policyNumber);
                throw;
            }
        }
    }
}
