using TagTheSpot.Services.Moderation.Application.Abstractions.Data;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Domain.Submissions;

namespace TagTheSpot.Services.Moderation.Application.Mappers
{
    public sealed class SubmissionToSubmissionResponseMapper
        : Mapper<Submission, SubmissionResponse>
    {
        public override SubmissionResponse Map(Submission source)
        {
            return new SubmissionResponse(
                Id: source.Id,
                UserId: source.UserId,
                CityId: source.CityId,
                CityName: source.CityName,
                Latitude: source.Latitude,
                Longitude: source.Longitude,
                SpotType: source.SpotType.ToString(),
                Description: source.Description,
                SkillLevel: source.SkillLevel.ToString(),
                IsCovered: source.IsCovered,
                Lighting: source.Lighting,
                Accessibility: source.Accessibility.ToString(),
                Condition: source.Condition.ToString(),
                ImagesUrls: source.ImagesUrls,
                SubmissionStatus: source.Status.ToString(),
                RejectionReason: source.RejectionReason,
                SubmittedAt: source.SubmittedAt);
        }
    }
}
