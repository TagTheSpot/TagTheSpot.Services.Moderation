namespace TagTheSpot.Services.Moderation.Domain.Submissions
{
    public interface ISubmissionRepository
    {
        Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Submission>> GetWithStatusPendingAsync(
            CancellationToken cancellationToken = default);
    }
}
