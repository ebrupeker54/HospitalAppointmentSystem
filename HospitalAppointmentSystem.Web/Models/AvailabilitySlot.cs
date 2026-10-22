namespace HospitalAppointmentSystem.Web.Models;

public class AvailabilitySlot
{
    public int Id { get; private set; }
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }

    public int ProfessionalId { get; set; }
    public HealthcareProfessional? Professional { get; set; }

    // Required by Entity Framework Core
    private AvailabilitySlot() { }

    public AvailabilitySlot(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        Start = start;
        End = end;
    }

    public bool Contains(DateTime start, DateTime end) => start >= Start && end <= End;

    public bool Overlaps(AvailabilitySlot other) => Start < other.End && other.Start < End;
}