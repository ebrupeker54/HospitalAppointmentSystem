namespace HospitalAppointmentSystem.Web.Models;

public abstract class HealthcareProfessional : Staff
{
    public List<MedicalService> Services { get; private set; } = new();
    public List<AvailabilitySlot> Availability { get; private set; } = new();

    public abstract string Title { get; }

    public string DisplayName => $"{Title} {FullName}";

    public override string DashboardController => "Professional";

    public abstract bool CanOffer(MedicalService service);

    public void AddService(MedicalService service)
    {
        if (!CanOffer(service))
            throw new InvalidOperationException($"{Title} bu hizmeti veremez: {service.Name}");

        if (Branch?.Institution != null && service.Institution != null
            && Branch.Institution != service.Institution)
            throw new InvalidOperationException("Sağlık personeli yalnızca kendi kurumunun hizmetlerini verebilir.");

        if (Services.Contains(service))
            return;

        Services.Add(service);
        service.Professionals.Add(this);
    }

    public bool CanPerform(MedicalService service) => Services.Contains(service);

    public void SetServices(IEnumerable<MedicalService> services)
    {
        foreach (var existing in Services.ToList())
        {
            Services.Remove(existing);
            existing.Professionals.Remove(this);
        }

        foreach (var service in services)
            AddService(service);
    }

    public void AddAvailability(AvailabilitySlot slot)
    {
        if (Branch != null && !Branch.IsOpenBetween(slot.Start, slot.End))
            throw new InvalidOperationException("Uygunluk aralığı şubenin çalışma saatleri içinde olmalıdır.");

        if (Availability.Any(s => s.Overlaps(slot)))
            throw new InvalidOperationException("Bu aralık mevcut bir uygunluk aralığıyla çakışıyor.");

        slot.Professional = this;
        Availability.Add(slot);
    }

    public bool IsAvailable(DateTime start, DateTime end) =>
        Availability.Any(s => s.Contains(start, end));
}