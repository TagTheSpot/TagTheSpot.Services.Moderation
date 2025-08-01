using TagTheSpot.Services.Moderation.Application.Abstractions.Data;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Domain.Submissions;

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
    }
}
