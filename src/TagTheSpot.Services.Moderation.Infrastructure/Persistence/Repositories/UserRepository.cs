using TagTheSpot.Services.Moderation.Domain.Users;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FindAsync(
                keyValues: [id],
                cancellationToken: cancellationToken);
        }

        public async Task InsertAsync(
            User user,
            CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(
                user, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
