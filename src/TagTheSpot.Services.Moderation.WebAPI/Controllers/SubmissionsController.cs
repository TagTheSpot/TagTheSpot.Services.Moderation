using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.WebAPI.Factories;

namespace TagTheSpot.Services.Moderation.WebAPI.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public SubmissionsController(
            ISubmissionService submissionService, 
            ProblemDetailsFactory problemDetailsFactory)
        {
            _submissionService = submissionService;
            _problemDetailsFactory = problemDetailsFactory;
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

        [Authorize(Roles = "Admin,Owner")]
        [HttpPatch("reject")]
        public async Task<IActionResult> RejectSubmission(
            [FromBody] RejectSubmissionRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _submissionService
                .RejectAsync(request, cancellationToken);

            return result.IsSuccess ? NoContent() : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
