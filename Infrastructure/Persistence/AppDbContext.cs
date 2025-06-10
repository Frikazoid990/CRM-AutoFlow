using CRM_AutoFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CRM_AutoFlow.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //DbSet<МодельМоя> Свойство к которому я буду обращаться/таблица в бд
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<TestDrive> TestDrives { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Deal> Deals { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
