using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HospitalAppointmentSystem.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store all dates as local clinic time (no time zone conversion)
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // All user types are stored in a single "Users" table, separated by Role
        modelBuilder.Entity<User>()
            .HasDiscriminator(u => u.Role)
            .HasValue<Patient>(UserRole.Patient)
            .HasValue<Doctor>(UserRole.Doctor)
            .HasValue<Admin>(UserRole.Admin);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Branch>()
            .HasMany(b => b.Schedule)
            .WithOne()
            .HasForeignKey(w => w.BranchId);

        modelBuilder.Entity<MedicalService>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<MedicalService>()
            .Property(s => s.Category)
            .HasConversion<string>();

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(a => a.Price).HasPrecision(10, 2);
            entity.Property(a => a.Status).HasConversion<string>();

            entity.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.MedicalService)
                .WithMany()
                .HasForeignKey(a => a.MedicalServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}