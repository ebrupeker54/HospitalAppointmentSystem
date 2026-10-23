namespace HospitalAppointmentSystem.Web.Models;

public abstract class Staff : User
{
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public abstract string Title { get; }

    public string DisplayName => $"{Title} {FullName}";

    public void AssignTo(Branch branch, Department? department)
    {
        if (department != null && department.BranchId != branch.Id)
            throw new InvalidOperationException("Seçilen bölüm, seçilen şubeye ait değil.");

        Branch = branch;
        BranchId = branch.Id;
        Department = department;
        DepartmentId = department?.Id;
    }
}