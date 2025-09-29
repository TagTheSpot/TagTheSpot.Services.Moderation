using Microsoft.Extensions.Options;
using TagTheSpot.Services.Moderation.Application.Abstractions.Generators;
using TagTheSpot.Services.Moderation.Infrastructure.Options;

namespace TagTheSpot.Services.Moderation.Infrastructure.Services
{
    public sealed class SubmissionLinkGenerator : ISubmissionLinkGenerator
    {
        private readonly LinkGeneratorSettings _linkGeneratorSettings;

        public SubmissionLinkGenerator(
            IOptions<LinkGeneratorSettings> linkGeneratorSettings)
        {
            _linkGeneratorSettings = linkGeneratorSettings.Value;
        }

        public string Generate(Guid submissionId)
        {
            return _linkGeneratorSettings.SubmissionDetailsLink
                .Replace($"{{{_linkGeneratorSettings.SubmissionDetailsLinkPlaceholderName}}}",
                    newValue: submissionId.ToString());
        }
    }
}
