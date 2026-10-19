namespace HospitalAppointmentSystem.Web.Models;

public class Doctor : User
{
    public string Specialty { get; set; } = string.Empty;

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public List<MedicalService> Services { get; private set; } = new();
    public List<AvailabilitySlot> Availability { get; private set; } = new();

    public Doctor()
    {
        Role = UserRole.Doctor;
    }
    public override string DashboardController => "Doctor";

    public void AddService(MedicalService service)
    {
        if (Branch?.Institution != null && service.Institution != null
            && Branch.Institution != service.Institution)
            throw new InvalidOperationException("A doctor can only offer services of their own institution.");

        if (Services.Contains(service))
            return;

        Services.Add(service);
        service.Doctors.Add(this);
    }

    public bool CanPerform(MedicalService service) => Services.Contains(service);

    public void AddAvailability(AvailabilitySlot slot)
    {
        if (Branch != null && !Branch.IsOpenBetween(slot.Start, slot.End))
            throw new InvalidOperationException("The slot must be within the branch's working hours.");

        if (Availability.Any(s => s.Overlaps(slot)))
            throw new InvalidOperationException("This slot overlaps with an existing availability slot.");

        slot.Doctor = this;
        Availability.Add(slot);
    }

    public bool IsAvailable(DateTime start, DateTime end) =>
        Availability.Any(s => s.Contains(start, end));

    public override string GetRoleDescription()
    {
        return $"Doctor ({Specialty}) - can manage availability and approve appointments";
    }
}