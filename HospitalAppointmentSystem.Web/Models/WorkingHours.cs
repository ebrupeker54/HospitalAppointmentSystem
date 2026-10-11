namespace HospitalAppointmentSystem.Web.Models;

public class WorkingHours
{
    public int Id { get; private set; }
    public DayOfWeek Day { get; private set; }
    public TimeOnly OpenTime { get; private set; }
    public TimeOnly CloseTime { get; private set; }

    public int BranchId { get; set; }

    // Required by Entity Framework Core
    private WorkingHours() { }

    public WorkingHours(DayOfWeek day, TimeOnly openTime, TimeOnly closeTime)
    {
        if (closeTime <= openTime)
            throw new ArgumentException("Closing time must be after opening time.");

        Day = day;
        OpenTime = openTime;
        CloseTime = closeTime;
    }

    public bool Covers(TimeOnly time) => time >= OpenTime && time < CloseTime;
}