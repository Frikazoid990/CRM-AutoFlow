using CRM_AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_AutoFlow.Infrastructure.Persistence.ConfigurationsModelsToDb
{
    public class TestDriveConfiguration : IEntityTypeConfiguration<TestDrive>
    {
        public void Configure(EntityTypeBuilder<TestDrive> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.PlannedDate)
                .IsRequired();
            
            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(t => t.Client)
                .WithMany(u => u.ClientTestDrives)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasOne(t => t.Employee)
                .WithMany(u => u.EmployeeTestDrives)
                .HasForeignKey(t => t.EmployeedId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.Car)
                .WithMany(c => c.TestDrives)
                .HasForeignKey(t => t.CarId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
