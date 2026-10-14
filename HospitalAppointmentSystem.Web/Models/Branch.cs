namespace HospitalAppointmentSystem.Web.Models;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public int InstitutionId { get; set; }
    public Institution? Institution { get; set; }

    public List<Department> Departments { get; private set; } = new();
    public List<WorkingHours> Schedule { get; private set; } = new();

    public void AddDepartment(Department department)
    {
        if (Departments.Any(d => string.Equals(d.Name, department.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Department '{department.Name}' already exists in this branch.");

        department.Branch = this;
        Departments.Add(department);
    }

    public void SetWorkingHours(WorkingHours hours)
    {
        Schedule.RemoveAll(w => w.Day == hours.Day);
        Schedule.Add(hours);
    }

    public bool IsOpenAt(DateTime dateTime)
    {
        var hours = Schedule.FirstOrDefault(w => w.Day == dateTime.DayOfWeek);
        return hours != null && hours.Covers(TimeOnly.FromDateTime(dateTime));
    }
    public bool IsOpenBetween(DateTime start, DateTime end)
    {
        if (start.Date != end.Date)
            return false;

        var hours = Schedule.FirstOrDefault(w => w.Day == start.DayOfWeek);
        return hours != null
            && TimeOnly.FromDateTime(start) >= hours.OpenTime
            && TimeOnly.FromDateTime(end) <= hours.CloseTime;
    }
}