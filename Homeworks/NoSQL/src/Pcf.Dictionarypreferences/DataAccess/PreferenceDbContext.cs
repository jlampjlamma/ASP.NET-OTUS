using Microsoft.EntityFrameworkCore;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace DataAccess;

public class PreferenceDbContext : DbContext
{
    public PreferenceDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Preference> Preferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Preference>(builder =>
        {
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        });
    }
}
