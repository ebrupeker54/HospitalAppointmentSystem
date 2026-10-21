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
            throw new InvalidOperationException("Doktor yalnızca kendi kurumunun hizmetlerini verebilir.");

        if (Services.Contains(service))
            return;

        Services.Add(service);
        service.Doctors.Add(this);
    }

    public bool CanPerform(MedicalService service) => Services.Contains(service);

    public void AssignTo(Branch branch, Department? department)
    {
        if (department != null && department.BranchId != branch.Id)
            throw new InvalidOperationException("Seçilen bölüm, seçilen şubeye ait değil.");

        Branch = branch;
        BranchId = branch.Id;
        Department = department;
        DepartmentId = department?.Id;
    }

    public void SetServices(IEnumerable<MedicalService> services)
    {
        foreach (var existing in Services.ToList())
        {
            Services.Remove(existing);
            existing.Doctors.Remove(this);
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

        slot.Doctor = this;
        Availability.Add(slot);
    }

    public bool IsAvailable(DateTime start, DateTime end) =>
        Availability.Any(s => s.Contains(start, end));

    public override string GetRoleDescription()
    {
        return $"Doktor ({Specialty}) - uygunluk saatlerini yönetebilir ve randevuları onaylayabilir";
    }
}