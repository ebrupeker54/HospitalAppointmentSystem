namespace HospitalAppointmentSystem.Web.Models;

public class Appointment
{
    public int Id { get; private set; }

    public int PatientId { get; private set; }
    public Patient? Patient { get; private set; }

    public int DoctorId { get; private set; }
    public Doctor? Doctor { get; private set; }

    public int MedicalServiceId { get; private set; }
    public MedicalService? MedicalService { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }

    public AppointmentStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public TimeSpan Duration => EndTime - StartTime;
    public bool IsActive => Status == AppointmentStatus.Pending || Status == AppointmentStatus.Approved;

    // Required by Entity Framework Core
    private Appointment() { }

    public Appointment(Patient patient, Doctor doctor, MedicalService service, DateTime startTime)
    {
        if (startTime <= DateTime.Now)
            throw new ArgumentException("Appointment must be in the future.");
        if (!doctor.CanPerform(service))
            throw new InvalidOperationException("This doctor does not offer the selected service.");

        Patient = patient;
        PatientId = patient.Id;
        Doctor = doctor;
        DoctorId = doctor.Id;
        MedicalService = service;
        MedicalServiceId = service.Id;

        StartTime = startTime;
        EndTime = startTime + service.Duration;
        Price = service.Price;

        Status = AppointmentStatus.Pending;
        CreatedAt = DateTime.Now;
    }

    public bool Overlaps(DateTime start, DateTime end) => start < EndTime && StartTime < end;

    public void Approve()
    {
        EnsureStatus(AppointmentStatus.Pending, "approved");
        Status = AppointmentStatus.Approved;
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A rejection reason is required.");

        EnsureStatus(AppointmentStatus.Pending, "rejected");
        Status = AppointmentStatus.Rejected;
        RejectionReason = reason.Trim();
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new InvalidOperationException("Only pending or approved appointments can be cancelled.");
        Status = AppointmentStatus.Cancelled;
    }

    public void Complete()
    {
        EnsureStatus(AppointmentStatus.Approved, "completed");
        Status = AppointmentStatus.Completed;
    }

    private void EnsureStatus(AppointmentStatus expected, string action)
    {
        if (Status != expected)
            throw new InvalidOperationException($"Only {expected} appointments can be {action}. Current status: {Status}.");
    }
}