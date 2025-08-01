namespace TagTheSpot.Services.Moderation.Application.DTO.UseCases
{
    public sealed record RejectSubmissionRequest(
        Guid SubmissionId,
        string RejectionReason);
}
