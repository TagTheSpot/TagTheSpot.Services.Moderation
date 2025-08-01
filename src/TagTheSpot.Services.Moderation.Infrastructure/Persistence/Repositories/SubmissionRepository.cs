using Microsoft.EntityFrameworkCore;
using TagTheSpot.Services.Moderation.Domain.Submissions;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories
{
    public sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubmissionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Submission>> GetWithStatusPendingAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions
                .Where(sub => sub.Status == SubmissionStatus.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Submissions.AddAsync(
                submission, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);    
        }
    }
}
