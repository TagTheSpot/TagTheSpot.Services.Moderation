using MassTransit;
using TagTheSpot.Services.Moderation.Application.Abstractions.Data;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;

namespace TagTheSpot.Services.Moderation.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly Mapper<Submission, SubmissionResponse> _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            Mapper<Submission, SubmissionResponse> mapper,
            IPublishEndpoint publishEndpoint)
        {
            _submissionRepository = submissionRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<IEnumerable<SubmissionResponse>> GetPendingSubmissionsAsync(
            CancellationToken cancellationToken = default)
        {
            var pendingSubmissions = await _submissionRepository
                .GetWithStatusPendingAsync(cancellationToken);

            return _mapper.Map(pendingSubmissions);
        }

        public async Task<Result> ApproveAsync(
            Guid submissionId, 
            CancellationToken cancellationToken = default)
        {
            var submission = await _submissionRepository.GetByIdAsync(
                submissionId, cancellationToken);

            if (submission is null)
            {
                return Result.Failure(SubmissionErrors.NotFound);
            }

            if (submission.Status != SubmissionStatus.Pending)
            {
                return Result.Failure(
                    SubmissionErrors.NotInPendingStatus(currentStatus: submission.Status.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(submission.RejectionReason))
            {
                throw new InvalidOperationException($"The rejection reason is not empty even though the status is Pending. Submission Id: {submissionId}");
            }

            submission.Status = SubmissionStatus.Approved;

            await _submissionRepository.UpdateAsync(
                submission, cancellationToken);

            await _publishEndpoint.Publish(
                new SubmissionApprovedEvent(submissionId));

            return Result.Success();
        }

        public async Task<Result> RejectAsync(
            RejectSubmissionRequest request, 
            CancellationToken cancellationToken = default)
        {
            var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);

            if (submission is null)
            {
                return Result.Failure(SubmissionErrors.NotFound);
            }

            if (submission.Status != SubmissionStatus.Pending)
            {
                return Result.Failure(
                    SubmissionErrors.NotInPendingStatus(currentStatus: submission.Status.ToString()));
            }

            submission.RejectionReason = request.RejectionReason;
            submission.Status = SubmissionStatus.Rejected;

            await _submissionRepository.UpdateAsync(
                submission, cancellationToken);

            await _publishEndpoint.Publish(
                new SubmissionRejectedEvent(
                    SubmissionId: submission.Id,
                    RejectionReason: submission.RejectionReason));

            return Result.Success();
        }
    }
}
