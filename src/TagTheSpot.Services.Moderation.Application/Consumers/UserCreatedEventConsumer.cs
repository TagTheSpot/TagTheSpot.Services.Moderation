using MassTransit;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Moderation.Domain.Users;
using TagTheSpot.Services.Shared.Messaging.Users;

namespace TagTheSpot.Services.Moderation.Application.Consumers
{
    public sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserCreatedEventConsumer> _logger;

        public UserCreatedEventConsumer(
            IUserRepository userRepository,
            ILogger<UserCreatedEventConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Consuming UserCreatedEvent for UserId: {UserId}, Email: {Email}", message.UserId, message.Email);

            try
            {
                var user = new User()
                {
                    Email = message.Email,
                    Id = message.UserId,
                    Role = Enum.Parse<Role>(message.Role)
                };

                await _userRepository.InsertAsync(user, context.CancellationToken);

                _logger.LogInformation("User with UserId: {UserId} inserted successfully.", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming UserCreatedEvent for UserId: {UserId}", message.UserId);

                throw;
            }
        }
    }
}
