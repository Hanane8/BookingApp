using Booking.Database.Entities;
using Microsoft.EntityFrameworkCore;



namespace Booking.Database.Database
{
    public class BokningContext : DbContext
    {
        public DbSet<Concert> Concerts { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Bokning> Boknings { get; set; }
        public DbSet<User> Users { get; set; }

        public BokningContext(DbContextOptions<BokningContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Concert>().HasKey(c => c.Id);
            modelBuilder.Entity<Performance>().HasKey(p => p.Id);
            modelBuilder.Entity<Bokning>().HasKey(b => b.Id);
            modelBuilder.Entity<User>().HasKey(c => c.Id);

            modelBuilder.Entity<Performance>()
                .HasOne(p => p.Concert)
                .WithMany(c => c.Performances)
                .HasForeignKey(p => p.ConcertId);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Performance)
                .WithMany(p => p.Boknings)
                .HasForeignKey(b => b.PerformanceId);

            // Lägg till relation mellan Bokning och User
            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

        }
    }
}
