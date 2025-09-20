using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TagTheSpot.Services.Moderation.Domain.Users;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence.Configurations
{
    internal sealed class UserConfiguration
        : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.Role)
                   .HasConversion<string>()
                   .IsRequired();
        }
    }
}
