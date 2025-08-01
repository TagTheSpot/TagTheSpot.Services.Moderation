using Microsoft.EntityFrameworkCore;
using TagTheSpot.Services.Moderation.Domain.Submissions;
using TagTheSpot.Services.Moderation.Domain.Users;

namespace TagTheSpot.Services.Moderation.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Submission> Submissions { get; init; }

        public DbSet<User> Users { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
