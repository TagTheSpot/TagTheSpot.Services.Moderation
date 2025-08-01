using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;

namespace TagTheSpot.Services.Moderation.WebAPI.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingSubmissions(
            CancellationToken cancellationToken)
        {
            var pendingSubmissions = await _submissionService
                .GetPendingSubmissionsAsync(cancellationToken);

            return Ok(pendingSubmissions);
        }
    }
}
