namespace HospitalAppointmentSystem.Web.Models;

public class MedicalService
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ServiceCategory Category { get; private set; }
    public TimeSpan Duration { get; private set; }
    public decimal Price { get; private set; }

    public int InstitutionId { get; set; }
    public Institution? Institution { get; set; }

    public List<Doctor> Doctors { get; private set; } = new();

    // Required by Entity Framework Core
    private MedicalService() { }

    public MedicalService(string name, ServiceCategory category, TimeSpan duration, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service name cannot be empty.");

        Name = name;
        Category = category;
        UpdatePricing(duration, price);
    }

    public void UpdateDetails(string name, ServiceCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service name cannot be empty.");

        Name = name.Trim();
        Category = category;
    }

    public void UpdatePricing(TimeSpan duration, decimal price)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero.");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        Duration = duration;
        Price = price;
    }
}