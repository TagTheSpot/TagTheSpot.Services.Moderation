using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Moderation.Infrastructure.Options
{
    public sealed class LinkGeneratorSettings
    {
        public const string SectionName = nameof(LinkGeneratorSettings);

        [Required]
        [Url]
        public required string SubmissionDetailsLink { get; init; }

        [Required]
        public required string SubmissionDetailsLinkPlaceholderName { get; init; }
    }
}
