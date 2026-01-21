using FindMyMeds.Core.Entities;
using FindMyMeds.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Medication> Medications => Set<Medication>();
        public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
        public DbSet<PharmacyMedication> PharmacyMedications => Set<PharmacyMedication>();
        public DbSet<MedicationSubstitute> MedicationSubstitutes => Set<MedicationSubstitute>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicationSubstitute>()
                .HasOne(ms => ms.PrimaryMedication)
                .WithMany(m => m.AsPrimarySubstitutes)
                .HasForeignKey(ms => ms.PrimaryMedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicationSubstitute>()
                .HasOne(ms => ms.AlternativeMedication)
                .WithMany(m => m.AsAlternativeSubstitutes)
                .HasForeignKey(ms => ms.AlternativeMedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PharmacyMedication>()
            .Property(pm => pm.Price)
            .HasPrecision(10, 2); 


        }
    }
}
