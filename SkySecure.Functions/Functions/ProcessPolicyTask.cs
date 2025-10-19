using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkySecure.Api.Models;
using SkySecure.Api.Services.Interfaces;
using SkySecure.Functions.Models;
using SkySecure.Functions.Services.Interfaces;
using System.Text.Json;

namespace SkySecure.Functions
{
    public class ProcessPolicyTask
    {
        private readonly ILogger<ProcessPolicyTask> _logger;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IEmailService _emailService;
        private readonly IInventoryService _inventory;

        public ProcessPolicyTask(ILogger<ProcessPolicyTask> logger,
                                 IPdfGenerator pdfGenerator,
                                 IEmailService emailService,
                                 IInventoryService inventory)
        {
            _logger = logger;
            _pdfGenerator = pdfGenerator;
            _emailService = emailService;
            _inventory = inventory;
        }

        [Function("ProcessPolicyTask")]
        public async Task Run([QueueTrigger("policy-tasks", Connection = "AzureWebJobsStorage")] string msg)
        {
            _logger.LogInformation("Processing policy task...");
            var request = JsonSerializer.Deserialize<PolicyData>(msg);
            string policyNumber = request!.PolicyNumber;
            decimal premium = request.Premium;

            var pdfPath = await _pdfGenerator.GeneratePdfAsync(request.PolicyRequest, policyNumber, premium);
            await _emailService.SendPolicyEmailAsync(request.PolicyRequest.ClientEmail, pdfPath, policyNumber);
            await _inventory.IncrementPolicyCountAsync(request.PolicyRequest.DroneModel);

            _logger.LogInformation("Finished processing policy {Policy}", policyNumber);
        }
    }
}
