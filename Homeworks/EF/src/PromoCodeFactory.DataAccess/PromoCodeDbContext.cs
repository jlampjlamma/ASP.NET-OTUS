using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System.Reflection;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(225);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(225);

            builder.Property(x => x.Email)
                .HasMaxLength(320);

            builder.Property(x => x.AppliedPromocodesCount);

            builder.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId);

            builder.HasData(FakeDataFactory.Employees);
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(225);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(225);

            builder.HasData(FakeDataFactory.Roles);
        });

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(225);

            builder.Property(x => x.LastName)
                .HasMaxLength(500);

            builder.Property(x => x.Email)
                .HasMaxLength(320);

            builder.HasMany(x => x.Preferences)
            .WithMany(x => x.Customers)
            .UsingEntity(x => x.ToTable("CustomerPreference"));


            builder.HasMany(x => x.PromoCode)
                .WithOne();

            builder.HasData(FakeDataFactory.Customers);
        });

        modelBuilder.Entity<Preference>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(225);

            builder.HasData(FakeDataFactory.Preferences);
        });

        modelBuilder.Entity<PromoCode>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(225);

            builder.Property(x => x.ServiceInfo)
            .IsRequired()
            .HasMaxLength(225);

            //builder.Property(x => x.PartnerName)
            //.IsRequired()
            //.HasMaxLength(225);

            builder.Property(x => x.BeginDate)
            .IsRequired();

            builder.Property(x => x.EndDate)
            .IsRequired();

            builder.HasOne(x => x.Preference)
            .WithMany()
            .HasForeignKey(x => x.PreferenceId);

            builder.HasOne(x => x.PartnerManager)
            .WithMany()
            .HasForeignKey(x => x.PartnerManagerId);
        });
    }
}
