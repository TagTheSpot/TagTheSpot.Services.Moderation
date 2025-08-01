using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.Moderation.Application.Abstractions.Data;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.Consumers;
using TagTheSpot.Services.Moderation.Application.DTO.UseCases;
using TagTheSpot.Services.Moderation.Application.Mappers;
using TagTheSpot.Services.Moderation.Application.Services;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Moderation.Domain.Users;
using TagTheSpot.Services.Moderation.Infrastructure.Extensions;
using TagTheSpot.Services.Moderation.Infrastructure.Options;
using TagTheSpot.Services.Moderation.Infrastructure.Persistence;
using TagTheSpot.Services.Moderation.Infrastructure.Persistence.Options;
using TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories;
using TagTheSpot.Services.Moderation.WebAPI.Extensions;
using TagTheSpot.Services.Moderation.WebAPI.Factories;
using TagTheSpot.Services.Moderation.WebAPI.Middleware;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;
using TagTheSpot.Services.Shared.Messaging.Events.Users;
using TagTheSpot.Services.Shared.Messaging.Options;

namespace TagTheSpot.Services.Moderation.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwaggerGen();

            builder.Services.AddOptions<DbSettings>()
                .BindConfiguration(DbSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<RabbitMqSettings>()
                .BindConfiguration(RabbitMqSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration(JwtSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<MessagingSettings>()
                .BindConfiguration(MessagingSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.ConfigureAuthentication();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<ProblemDetailsFactory>();

            builder.Services.AddDbContext<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;

                    options.UseNpgsql(dbSettings.ConnectionString);
                });

            builder.Services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SpotSubmittedEventConsumer>();
                cfg.AddConsumer<UserCreatedEventConsumer>();

                cfg.UsingRabbitMq((context, config) =>
                {
                    var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                    var messagingSettings = context.GetRequiredService<IOptions<MessagingSettings>>().Value;

                    config.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    config.ReceiveEndpoint(messagingSettings.QueueName, e =>
                    {
                        e.Bind<SpotSubmittedEvent>();
                        e.ConfigureConsumer<SpotSubmittedEventConsumer>(context);

                        e.Bind<UserCreatedEvent>();
                        e.ConfigureConsumer<UserCreatedEventConsumer>(context);
                    });
                });
            });

            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<Mapper<Submission, SubmissionResponse>, SubmissionToSubmissionResponseMapper>();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.ApplyMigrations();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
