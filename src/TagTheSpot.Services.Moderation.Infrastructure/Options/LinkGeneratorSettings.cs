using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.Moderation.Infrastructure.Options
{
    public sealed class LinkGeneratorSettings : IAppOptions
    {
        public static string SectionName => nameof(LinkGeneratorSettings);

        [Required]
        [Url]
        public required string SubmissionDetailsLink { get; init; }

        [Required]
        public required string SubmissionDetailsLinkPlaceholderName { get; init; }
    }
}
