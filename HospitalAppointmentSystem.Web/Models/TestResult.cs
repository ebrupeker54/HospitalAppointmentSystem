namespace HospitalAppointmentSystem.Web.Models;

public class TestResult
{
    public int Id { get; private set; }

    public int AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }

    public int AuthorId { get; private set; }
    public HealthcareProfessional? Author { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Findings { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // Required by Entity Framework Core
    private TestResult() { }

    public TestResult(Appointment appointment, HealthcareProfessional author, string title, string findings)
    {
        if (appointment.ProfessionalId != author.Id)
            throw new InvalidOperationException("Yalnızca kendi randevularınız için sonuç girebilirsiniz.");
        if (appointment.Status != AppointmentStatus.Completed)
            throw new InvalidOperationException("Sonuç yalnızca tamamlanmış randevular için girilebilir.");
        if (string.IsNullOrWhiteSpace(findings))
            throw new ArgumentException("Sonuç metni boş olamaz.");

        Appointment = appointment;
        AppointmentId = appointment.Id;
        Author = author;
        AuthorId = author.Id;
        Title = title;
        Findings = findings.Trim();
        CreatedAt = DateTime.Now;
    }
}