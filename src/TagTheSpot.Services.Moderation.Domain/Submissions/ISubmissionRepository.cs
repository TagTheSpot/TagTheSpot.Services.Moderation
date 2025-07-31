using TagTheSpot.Services.Moderation.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public interface ISubmissionRepository
    {
        Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken);
    }
}
