namespace TagTheSpot.Services.Moderation.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; init; }

        public required string Email { get; set; }

        public Role Role { get; set; }
    }
}
