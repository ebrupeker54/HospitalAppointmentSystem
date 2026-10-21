namespace HospitalAppointmentSystem.Web.ViewModels;

public class DayScheduleInput
{
    public DayOfWeek Day { get; set; }
    public bool IsOpen { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
}