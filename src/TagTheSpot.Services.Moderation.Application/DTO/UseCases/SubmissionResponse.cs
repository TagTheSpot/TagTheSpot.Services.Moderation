namespace TagTheSpot.Services.Moderation.Application.DTO.UseCases
{
    public sealed record SubmissionResponse(
        Guid Id,
        Guid UserId,
        Guid CityId,
        string CityName,
        double Latitude,
        double Longitude,
        string SpotType,
        string Description,
        string? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        string? Accessibility,
        string? Condition,
        List<string> ImagesUrls,
        string SubmissionStatus,
        string? RejectionReason,
        DateTime SubmittedAt);
}
