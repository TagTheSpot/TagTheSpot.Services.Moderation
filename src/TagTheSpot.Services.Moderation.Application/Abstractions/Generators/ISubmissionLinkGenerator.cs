namespace TagTheSpot.Services.Moderation.Application.Abstractions.Generators
{
    public interface ISubmissionLinkGenerator
    {
        string Generate(Guid submissionId);
    }
}
