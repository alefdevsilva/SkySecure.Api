using SkySecure.Api.Models;
using SkySecure.Api.Services.Interfaces;
using System.Text.Json;

namespace SkySecure.Api.Services
{
    public class PolicyIssuanceOrchestrator
    {
        private readonly IPilotValidationService _pilotValidator;
        private readonly IPolicyRepository _repo;
        private readonly IPolicyNumberProvider _numberProvider;
        private readonly IAzureQueueService _queue;
        private readonly ILogger<PolicyIssuanceOrchestrator> _logger;

        public PolicyIssuanceOrchestrator(
            IPilotValidationService pilotValidator,
            IPolicyRepository repo,
            IPolicyNumberProvider numberProvider,
            IAzureQueueService queue,
            ILogger<PolicyIssuanceOrchestrator> logger)
        {
            _pilotValidator = pilotValidator;
            _repo = repo;
            _numberProvider = numberProvider;
            _queue = queue;
            _logger = logger;
        }

        public async Task<PolicyResponse> IssuePolicyAsync(PolicyRequest request)
        {
            var response = new PolicyResponse();

            try
            {
                var resultValidation = await _pilotValidator.ValidateAsync(request.PilotDocument);
                if (!resultValidation.IsFailure)
                {
                    response.Success = false;
                    response.ErrorMessage = "Invalid pilot certification";
                    return response;
                }

                var risk = CalculateRisk(request);
                var premium = CalculatePremium(risk, request.DroneValue);
                var policyNumber = _numberProvider.GeneratePolicyNumber();

                //await _repo.SavePolicyAsync(request, policyNumber, premium);

                // Enfileira tarefa para o Azure Functions
                var message = JsonSerializer.Serialize(new
                {
                    PolicyNumber = policyNumber,
                    Request = request,
                    Premium = premium
                });
                await _queue.EnqueueMessageAsync(message);

                response.Success = true;
                response.PolicyNumber = policyNumber;
                response.Premium = premium;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error issuing policy");
                response.Success = false;
                response.ErrorMessage = "Internal server error";
            }

            return response;
        }

        private RiskData CalculateRisk(PolicyRequest request)
        {
            var r = new RiskData();
            if (request.DroneValue > 50000) r.RiskLevel = "HIGH";
            else if (request.DroneValue > 20000) r.RiskLevel = "MEDIUM";
            else r.RiskLevel = "LOW";

            var highRiskAreas = new[] { "SP", "RJ", "DF" };
            r.RiskMultiplier = highRiskAreas.Contains(request.OperationState) ? 1.5 : 1.0;
            return r;
        }

        private decimal CalculatePremium(RiskData risk, decimal droneValue)
        {
            decimal baseRate = risk.RiskLevel switch
            {
                "HIGH" => 0.05m,
                "MEDIUM" => 0.03m,
                _ => 0.02m
            };
            return droneValue * baseRate * (decimal)risk.RiskMultiplier;
        }
    }
}
