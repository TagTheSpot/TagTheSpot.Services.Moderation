using TagTheSpot.Services.Moderation.Application.DTO.UseCases;

namespace TagTheSpot.Services.Moderation.Application.Abstractions.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<SubmissionResponse>> GetPendingSubmissionsAsync(
            CancellationToken cancellationToken = default);
    }
}
