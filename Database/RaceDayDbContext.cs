using Microsoft.EntityFrameworkCore;
using RaceDay.API.models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RaceDay.API.Data
{//start of namespace
    
    public class RaceDayDbContext : DbContext
    {//start of class

        // Constructor accepting dependency injection for database options (e.g., SQL Server connection)
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

        // DbSet tables mapped to database relational tables
        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .Property(e => e.Distance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.OrganiserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }//end of override void
    }//end of class
}//end of namespace