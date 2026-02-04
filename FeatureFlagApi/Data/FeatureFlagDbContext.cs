using Microsoft.EntityFrameworkCore;

public class FeatureFlagDbContext(DbContextOptions<FeatureFlagDbContext> options)
    : DbContext(options)
{
    public DbSet<FeatureFlag> FeatureFlags { get; set; }

    public DbSet<FeatureFlagOverride> FeatureFlagOverrides { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FeatureFlag>().HasIndex(ff => ff.Key).IsUnique();
    }
}
