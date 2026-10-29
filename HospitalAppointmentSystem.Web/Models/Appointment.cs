namespace HospitalAppointmentSystem.Web.Models;

public class Appointment
{
    public int Id { get; private set; }

    public int PatientId { get; private set; }
    public Patient? Patient { get; private set; }

    public int ProfessionalId { get; private set; }
    public HealthcareProfessional? Professional { get; private set; }

    public int MedicalServiceId { get; private set; }
    public MedicalService? MedicalService { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }

    public AppointmentStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public TestResult? Result { get; private set; }
    public TimeSpan Duration => EndTime - StartTime;
    public bool IsActive => Status == AppointmentStatus.Pending || Status == AppointmentStatus.Approved;

    // Required by Entity Framework Core
    private Appointment() { }

    public Appointment(Patient patient, HealthcareProfessional professional, MedicalService service, DateTime startTime)
    {
        if (startTime <= DateTime.Now)
            throw new ArgumentException("Randevu ileri bir tarih için alınmalıdır.");
        if (!professional.CanPerform(service))
            throw new InvalidOperationException("Seçilen sağlık personeli bu hizmeti vermiyor.");

        Patient = patient;
        PatientId = patient.Id;
        Professional = professional;
        ProfessionalId = professional.Id;
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
        EnsureStatus(AppointmentStatus.Pending, "Yalnızca onay bekleyen randevular onaylanabilir.");
        Status = AppointmentStatus.Approved;
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Ret gerekçesi zorunludur.");

        EnsureStatus(AppointmentStatus.Pending, "Yalnızca onay bekleyen randevular reddedilebilir.");
        Status = AppointmentStatus.Rejected;
        RejectionReason = reason.Trim();
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new InvalidOperationException("Yalnızca onay bekleyen veya onaylanmış randevular iptal edilebilir.");
        Status = AppointmentStatus.Cancelled;
    }

    public void Complete()
    {
        EnsureStatus(AppointmentStatus.Approved, "Yalnızca onaylanmış randevular tamamlanabilir.");

        if (StartTime > DateTime.Now)
            throw new InvalidOperationException("Randevu saati gelmeden tamamlanamaz.");

        Status = AppointmentStatus.Completed;
    }

    private void EnsureStatus(AppointmentStatus expected, string errorMessage)
    {
        if (Status != expected)
            throw new InvalidOperationException(errorMessage);
    }
}