using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Moderation.Infrastructure.Options
{
    public sealed class JwtSettings
    {
        public const string SectionName = nameof(JwtSettings);

        [Required]
        [StringLength(200)]
        public required string Issuer { get; init; }

        [Required]
        [StringLength(200)]
        public required string Audience { get; init; }

        [Required]
        public required string SecretKey { get; init; }
    }
}
