namespace HospitalAppointmentSystem.Web.Models;

public interface IAppointmentManager
{
    int Id { get; }

    bool CanManage(int professionalId);
}