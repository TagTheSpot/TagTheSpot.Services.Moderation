using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Moderation.Domain.Users;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Configurations
{
    internal sealed class SubmissionConfiguration
        : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(sub => sub.Id);

            builder.Property(sub => sub.CityName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(sub => sub.Description)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(s => s.SpotType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(s => s.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(s => s.SkillLevel)
                   .HasConversion<string>();

            builder.Property(s => s.Accessibility)
                   .HasConversion<string>();

            builder.Property(s => s.Condition)
                   .HasConversion<string>();

            builder.Property(s => s.RejectionReason)
                   .HasMaxLength(500);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(sub => sub.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
