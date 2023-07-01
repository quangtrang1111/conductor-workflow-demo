using Conductor.Api;
using Microsoft.AspNetCore.Mvc;

namespace ConductorWorkflowDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunningWorkflowController : ControllerBase
    {
        private readonly ILogger<RunningWorkflowController> _logger;

        public RunningWorkflowController(ILogger<RunningWorkflowController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Run([FromBody] string worflowName = "TestNumberWorkflow")
        {
            var worflowInput = new Dictionary<string, object>() { { "max", 9 } };
            var workflowResource = new WorkflowResourceApi(ConductorWorkflow.BASE_PATH);

            var result = workflowResource.StartWorkflow(worflowName, worflowInput);

            return Ok(new { executionId = result });
        }
    }
}