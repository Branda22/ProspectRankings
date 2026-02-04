using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Source> Sources => Set<Source>();
    public DbSet<Prospect> Prospects => Set<Prospect>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Prospect>(entity =>
        {
            entity.HasOne(p => p.Source)
                  .WithMany(s => s.Prospects)
                  .HasForeignKey(p => p.SourceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.SourceId, p.Rank }).IsUnique();
        });
    }
}
