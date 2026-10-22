using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<HealthcareProfessional> HealthcareProfessionals => Set<HealthcareProfessional>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<HealthStaff> HealthStaffMembers => Set<HealthStaff>();
    public DbSet<Secretary> Secretaries => Set<Secretary>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<TestResult> TestResults => Set<TestResult>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store all dates as local clinic time (no time zone conversion)
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // All user types share one "Users" table; the concrete class is stored in "UserType"
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Admin>("Admin")
            .HasValue<Doctor>("Doctor")
            .HasValue<Nurse>("Nurse")
            .HasValue<RadiologyTechnician>("RadiologyTechnician")
            .HasValue<LabTechnician>("LabTechnician")
             .HasValue<Secretary>("Secretary");

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

            entity.HasOne(a => a.Professional)
                .WithMany()
                .HasForeignKey(a => a.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.MedicalService)
                .WithMany()
                .HasForeignKey(a => a.MedicalServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasOne(r => r.Appointment)
                .WithOne(a => a.Result)
                .HasForeignKey<TestResult>(r => r.AppointmentId);

            entity.HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Secretary>()
            .HasMany(s => s.Doctors)
            .WithMany(d => d.Secretaries)
            .UsingEntity(j => j.ToTable("SecretaryDoctors"));
    }
}