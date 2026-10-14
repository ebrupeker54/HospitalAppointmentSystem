using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.Repositories;

public interface IAppointmentRepository
{
    Appointment? GetById(int id);
    IEnumerable<Appointment> GetActiveByDoctor(int doctorId);
    IEnumerable<Appointment> GetActiveByPatient(int patientId);
    void Add(Appointment appointment);
    void Update(Appointment appointment);
}