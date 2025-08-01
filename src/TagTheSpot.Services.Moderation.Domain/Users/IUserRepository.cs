namespace TagTheSpot.Services.Moderation.Domain.Users
{
    public interface IUserRepository
    {
        Task InsertAsync(
            User user, 
            CancellationToken cancellationToken);
    }
}
