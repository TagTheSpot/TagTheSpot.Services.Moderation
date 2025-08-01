using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Moderation.Domain.Submissions
{
    public static class SubmissionErrors
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "Submission.NotFound",
                description: "The submission has not been found.");

        public static Error NotInPendingStatus(string currentStatus) =>
            Error.Validation(
                code: "Submission.NotInPendingStatus",
                description: $"Unable to perform the operation, because the submission is not in the pending status. The status is {currentStatus}.");
    }
}
