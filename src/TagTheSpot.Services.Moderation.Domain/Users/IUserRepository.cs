namespace TagTheSpot.Services.Moderation.Domain.Users
{
    public interface IUserRepository
    {
        Task InsertAsync(
            User user, 
            CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
