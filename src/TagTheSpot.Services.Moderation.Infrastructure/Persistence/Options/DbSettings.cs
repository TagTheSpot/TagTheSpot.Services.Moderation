using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Options
{
    public sealed class DbSettings
    {
        public const string SectionName = nameof(DbSettings);

        [Required]
        public required string ConnectionString { get; init; }
    }
}
