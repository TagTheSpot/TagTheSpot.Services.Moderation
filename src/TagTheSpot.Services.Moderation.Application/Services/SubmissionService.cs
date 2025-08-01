using TagTheSpot.Services.Moderation.Application.Abstractions.Data;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Moderation.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly Mapper<Submission, SubmissionResponse> _mapper;

        public SubmissionService(
            ISubmissionRepository submissionRepository, 
            Mapper<Submission, SubmissionResponse> mapper)
        {
            _submissionRepository = submissionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubmissionResponse>> GetPendingSubmissionsAsync(
            CancellationToken cancellationToken = default)
        {
            var pendingSubmissions = await _submissionRepository
                .GetWithStatusPendingAsync(cancellationToken);

            return _mapper.Map(pendingSubmissions);
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

            return Result.Success();
        }
    }
}
