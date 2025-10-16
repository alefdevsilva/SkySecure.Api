using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SkySecure.Api.Models;
using SkySecure.Api.Services;

namespace SkySecure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly PolicyIssuanceOrchestrator _orchestrator;
        private readonly ILogger<PolicyController> _logger;
        private readonly IConfiguration _config;

        public PolicyController(PolicyIssuanceOrchestrator orchestrator, ILogger<PolicyController> logger, IConfiguration config)
        {
            _orchestrator = orchestrator;
            _logger = logger;
            _config = config;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> Issue([FromBody] PolicyRequest req)
        {
            var res = await _orchestrator.IssuePolicyAsync(req);
            return res.Success ? Ok(res) : StatusCode(500, res);
        }
    }
}
