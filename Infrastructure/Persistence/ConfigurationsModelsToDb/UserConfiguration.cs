using CRM_AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_AutoFlow.Infrastructure.Persistence.ConfigurationsModelsToDb
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //PK
            builder.HasKey( f => f.Id );
            //Propertys Fields
            
            builder.Property(f => f.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.PhoneNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            builder.HasIndex(f => f.PhoneNumber).IsUnique();

            builder.Property(f => f.PasswordHash)
                .IsRequired()
                .HasMaxLength(1000);
            
            builder.Property(f => f.Role)
                .IsRequired()
                .HasConversion(
                f => f.ToString(), //enum ==> string
                f => (Role)Enum.Parse(typeof(Role),f)); 

            //Связь
            ///Deal
            builder.HasMany(u => u.ClientDeals)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(u => u.EmployeeDeals)
                .WithOne(d => d.Employee)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
            ///Test-Drive
            builder.HasMany(u => u.ClientTestDrives)
                .WithOne(t => t.Client)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
           
            builder.HasMany(u => u.EmployeeTestDrives)
                .WithOne(t => t.Employee)
                .HasForeignKey(t => t.EmployeedId)
                .OnDelete(DeleteBehavior.SetNull);
            ///Chat
            builder.HasMany(u => u.ClientChats)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(u => u.EmployeeChats)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
            ///Message
            builder.HasMany(u => u.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
