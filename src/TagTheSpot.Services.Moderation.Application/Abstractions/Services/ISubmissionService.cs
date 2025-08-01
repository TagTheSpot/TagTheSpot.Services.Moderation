using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Moderation.Application.Abstractions.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<SubmissionResponse>> GetPendingSubmissionsAsync(
            CancellationToken cancellationToken = default);

        Task<Result> RejectAsync(
            RejectSubmissionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result> ApproveAsync(
            Guid submissionId,
            CancellationToken cancellationToken = default);
    }
}
