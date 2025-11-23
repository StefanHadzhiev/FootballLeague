using FootballLeague.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FootballLeague.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Match> Matches => Set<Match>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Teams");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name)
                      .IsRequired();
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.ToTable("Matches");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.PlayedOn)
                      .IsRequired();

                entity.HasOne(m => m.HomeTeam)
                      .WithMany(t => t.HomeMatches)
                      .HasForeignKey(m => m.HomeTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.AwayTeam)
                      .WithMany(t => t.AwayMatches)
                      .HasForeignKey(m => m.AwayTeamId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
