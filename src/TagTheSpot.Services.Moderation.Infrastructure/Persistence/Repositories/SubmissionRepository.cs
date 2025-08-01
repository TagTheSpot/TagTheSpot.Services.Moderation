using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Moderation.Domain.Submissions;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories
{
    public sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SubmissionRepository> _logger;

        public SubmissionRepository(
            ApplicationDbContext dbContext, 
            ILogger<SubmissionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Submission?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions
                .FindAsync(
                    keyValues: [id], 
                    cancellationToken: cancellationToken);
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

        public async Task UpdateAsync(
            Submission submission, 
            CancellationToken cancellationToken = default)
        {
            _dbContext.Submissions.Update(submission);

            var affected = await _dbContext.SaveChangesAsync(cancellationToken);

            if (affected == 0 || affected > 1)
            {
                _logger.LogWarning("Affected {AffectedRows} rows while updating a Submission entity with Id {SubmissionId}", affected, submission.Id.ToString());
            }
        }
    }
}
