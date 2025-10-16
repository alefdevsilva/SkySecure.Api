namespace SkySecure.Functions.Models;

public record PolicyRequest(
       string ClientName,
       string ClientEmail,
       string DroneModel,
       decimal DroneValue,
       string PilotDocument,
       string OperationState,
       string UsageType);
public class PolicyResponse
{
    public bool Success { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class RiskData
{
    public string RiskLevel { get; set; } = "LOW";
    public double RiskMultiplier { get; set; } = 1.0;
}
