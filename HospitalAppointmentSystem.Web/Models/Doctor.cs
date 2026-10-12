namespace HospitalAppointmentSystem.Web.Models;

public class Doctor : User
{
    public string Specialty { get; set; } = string.Empty;

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public List<MedicalService> Services { get; private set; } = new();

    public Doctor()
    {
        Role = UserRole.Doctor;
    }

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

    public override string GetRoleDescription()
    {
        return $"Doctor ({Specialty}) - can manage availability and approve appointments";
    }
}