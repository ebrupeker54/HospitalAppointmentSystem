namespace HospitalAppointmentSystem.Web.Models;

public interface IResultEntry
{
    string ResultTitle { get; }

    TestResult WriteResult(Appointment appointment, string findings);
}