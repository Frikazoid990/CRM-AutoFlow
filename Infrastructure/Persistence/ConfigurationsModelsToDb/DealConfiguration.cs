using CRM_AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_AutoFlow.Infrastructure.Persistence.ConfigurationsModelsToDb
{
    public class DealConfiguration : IEntityTypeConfiguration<Deal>
    {
        public void Configure(EntityTypeBuilder<Deal> builder)
        {
            // PK
            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired();

            builder.Property(d => d.IsCancelled)
                .IsRequired();

            // Relationships
            builder.HasOne(d => d.Client)
                .WithMany(u => u.ClientDeals)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Employee)
                .WithMany(u => u.EmployeeDeals)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // Nullable?

            builder.HasOne(d => d.Car)
                .WithMany(c => c.Deals)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.SetNull); // Nullable

            builder.HasOne(d => d.Chat)
                .WithOne(c => c.ChatDeal)
                .HasForeignKey<Deal>(d => d.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
