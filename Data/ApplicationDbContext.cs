using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using szpont.Models;

namespace szpont.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Promotor)
                .WithMany()
                .HasForeignKey(t => t.PromotorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Kierownik)
                .WithMany()
                .HasForeignKey(t => t.KierownikId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Dziekan)
                .WithMany()
                .HasForeignKey(t => t.DziekanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
