using MassTransit;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Moderation.Domain.Spots;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;

namespace TagTheSpot.Services.Moderation.Application.Consumers
{
    public sealed class SpotSubmittedEventConsumer : IConsumer<SpotSubmittedEvent>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ILogger<SpotSubmittedEventConsumer> _logger;

        public SpotSubmittedEventConsumer(
            ISubmissionRepository submissionRepository,
            ILogger<SpotSubmittedEventConsumer> logger)
        {
            _submissionRepository = submissionRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SpotSubmittedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Consuming SpotSubmittedEvent for SubmissionId: {SubmissionId}", message.SubmissionId);

            try
            {
                var submission = new Submission()
                {
                    Description = message.Description,
                    Id = message.SubmissionId,
                    CityId = message.CityId,
                    Latitude = message.Latitude,
                    Longitude = message.Longitude,
                    UserId = message.UserId,
                    SpotType = Enum.Parse<SpotType>(message.SpotType),
                    CityName = message.CityName,
                    Accessibility = message.Accessibility is null ? null : Enum.Parse<Accessibility>(message.Accessibility),
                    Condition = message.Condition is null ? null : Enum.Parse<Condition>(message.Condition),
                    SkillLevel = message.SkillLevel is null ? null : Enum.Parse<SkillLevel>(message.SkillLevel),
                    Lighting = message.Lighting,
                    IsCovered = message.IsCovered,
                    ImagesUrls = message.ImagesUrls,
                    Status = SubmissionStatus.Pending,
                    SubmittedAt = message.SubmittedAt
                };

                await _submissionRepository.InsertAsync(
                    submission, context.CancellationToken);

                _logger.LogInformation("Submission with SubmissionId: {SubmissionId} inserted successfully.", message.SubmissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming SpotSubmittedEvent for SubmissionId: {SubmissionId}", message.SubmissionId);

                throw;
            }
        }
    }
}
