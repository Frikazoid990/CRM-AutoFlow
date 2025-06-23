using CRM_AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_AutoFlow.Infrastructure.Persistence.ConfigurationsModelsToDb
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            ///PK
            builder.HasKey(c => c.Id);
            ///Propertys Filds
            builder.Property(c => c.Brand)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.ConfigurationsJson)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.ImgPath)
                .IsRequired()
                .HasMaxLength(400);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(4000);
            ///Связь
            builder.HasMany(c => c.Deals)
                .WithOne(d=>d.Car)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.TestDrives)
                .WithOne(td => td.Car)
                .HasForeignKey(td => td.CarId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
