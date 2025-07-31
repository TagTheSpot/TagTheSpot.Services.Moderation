using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories
{
    public sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubmissionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken)
        {
            await _dbContext.Submissions.AddAsync(
                submission, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);    
        }
    }
}
