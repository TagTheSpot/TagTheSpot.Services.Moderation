using MassTransit;
using TagTheSpot.Services.Moderation.Application.Abstractions.Generators;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Moderation.Domain.Users;
using TagTheSpot.Services.Shared.Abstractions.Mappers;
using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Shared.Messaging.Submissions;

namespace TagTheSpot.Services.Moderation.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly Mapper<Submission, SubmissionResponse> _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISubmissionLinkGenerator _submissionLinkGenerator;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            IUserRepository userRepository,
            Mapper<Submission, SubmissionResponse> mapper,
            IPublishEndpoint publishEndpoint,
            ISubmissionLinkGenerator submissionLinkGenerator)
        {
            _submissionRepository = submissionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _submissionLinkGenerator = submissionLinkGenerator;
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

            var user = await _userRepository.GetByIdAsync(
                id: submission.UserId,
                cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException("The submission's user has not been found. Possible data inconsistency.");
            }

            var submissionLink = _submissionLinkGenerator.Generate(
                submissionId: submission.Id);

            await _submissionRepository.UpdateAsync(
                submission, cancellationToken);

            var approvedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new SubmissionApprovedEvent(submissionId));

            Console.WriteLine(new SendSubmissionApprovedEmailRequestedEvent(
                    SubmissionId: submission.Id,
                    Recipient: user.Email,
                    SubmissionLink: submissionLink,
                    SubmittedAt: submission.SubmittedAt,
                    ApprovedAt: approvedAt));

            await _publishEndpoint.Publish(
                new SendSubmissionApprovedEmailRequestedEvent(
                    SubmissionId: submission.Id,
                    Recipient: user.Email,
                    SubmissionLink: submissionLink,
                    SubmittedAt: submission.SubmittedAt,
                    ApprovedAt: approvedAt));

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

            var user = await _userRepository.GetByIdAsync(
                id: submission.UserId,
                cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException("The submission's user has not been found. Possible data inconsistency.");
            }

            var submissionLink = _submissionLinkGenerator.Generate(
                submissionId: submission.Id);

            await _submissionRepository.UpdateAsync(
                submission, cancellationToken);

            var rejectedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new SubmissionRejectedEvent(
                    SubmissionId: submission.Id,
                    RejectionReason: submission.RejectionReason));

            Console.WriteLine(new SendSubmissionRejectedEmailRequestedEvent(
                    SubmissionId: submission.Id,
                    Recipient: user.Email,
                    SubmissionLink: submissionLink,
                    RejectionReason: submission.RejectionReason,
                    SubmittedAt: submission.SubmittedAt,
                    RejectedAt: rejectedAt));

            await _publishEndpoint.Publish(
                new SendSubmissionRejectedEmailRequestedEvent(
                    SubmissionId: submission.Id,
                    Recipient: user.Email,
                    SubmissionLink: submissionLink,
                    RejectionReason: submission.RejectionReason,
                    SubmittedAt: submission.SubmittedAt,
                    RejectedAt: rejectedAt));

            return Result.Success();
        }
    }
}
