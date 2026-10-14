namespace HospitalAppointmentSystem.Web.Services;

public class AppointmentConflictException : Exception
{
    public AppointmentConflictException(string message) : base(message) { }
}