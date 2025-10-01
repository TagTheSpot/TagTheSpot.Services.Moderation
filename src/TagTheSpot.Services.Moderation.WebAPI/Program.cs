using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using TagTheSpot.Services.Moderation.Application.Abstractions.Generators;
using TagTheSpot.Services.Moderation.Application.Abstractions.Services;
using TagTheSpot.Services.Moderation.Application.Consumers;
using TagTheSpot.Services.Moderation.Application.Mappers;
using TagTheSpot.Services.Moderation.Application.Services;
using TagTheSpot.Services.Moderation.Application.Validators;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Moderation.Domain.Users;
using TagTheSpot.Services.Moderation.Infrastructure.Extensions;
using TagTheSpot.Services.Moderation.Infrastructure.Options;
using TagTheSpot.Services.Moderation.Infrastructure.Persistence;
using TagTheSpot.Services.Moderation.Infrastructure.Persistence.Repositories;
using TagTheSpot.Services.Moderation.Infrastructure.Services;
using TagTheSpot.Services.Shared.API.DependencyInjection;
using TagTheSpot.Services.Shared.API.Factories;
using TagTheSpot.Services.Shared.API.Middleware;
using TagTheSpot.Services.Shared.Application.Extensions;
using TagTheSpot.Services.Shared.Auth.DependencyInjection;
using TagTheSpot.Services.Shared.Auth.Options;
using TagTheSpot.Services.Shared.Infrastructure.Options;
using TagTheSpot.Services.Shared.Messaging.Submissions;
using TagTheSpot.Services.Shared.Messaging.Users;

namespace TagTheSpot.Services.Moderation.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var swaggerXmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var swaggerXmlFilePath = Path.Combine(AppContext.BaseDirectory, swaggerXmlFileName);

            builder.Services.ConfigureSwaggerGen(swaggerXmlFilePath);

            builder.Services.ConfigureValidatableOnStartOptions<RabbitMqSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<JwtSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<DbSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<MessagingSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<LinkGeneratorSettings>();

            builder.Services.AddValidatorsFromAssemblyContaining<RejectSubmissionRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation();

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
                cfg.SetKebabCaseEndpointNameFormatter();

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

            builder.Services.AddScoped<ISubmissionLinkGenerator, SubmissionLinkGenerator>();

            builder.Services.AddMappersFromAssembly(typeof(SubmissionToSubmissionResponseMapper).Assembly);

            builder.Services.AddDevelopmentCorsPolicy();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.ApplyMigrations();

            app.Run();
        }
    }
}
