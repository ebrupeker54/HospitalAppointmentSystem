using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.Repositories;

namespace HospitalAppointmentSystem.Web.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _repository;

    public AppointmentService(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public List<DateTime> GetAvailableStartTimes(HealthcareProfessional professional, MedicalService service, DateTime date)
    {
        var result = new List<DateTime>();
        if (professional.Branch == null || !professional.CanPerform(service))
            return result;

        var booked = _repository.GetActiveByProfessional(professional.Id).ToList();

        foreach (var slot in professional.Availability.Where(s => s.Start.Date == date.Date).OrderBy(s => s.Start))
        {
            for (var start = slot.Start; start + service.Duration <= slot.End; start += service.Duration)
            {
                var end = start + service.Duration;

                if (start <= DateTime.Now) continue;
                if (!professional.Branch.IsOpenBetween(start, end)) continue;
                if (booked.Any(a => a.Overlaps(start, end))) continue;

                result.Add(start);
            }
        }

        return result;
    }

    public Appointment Book(Patient patient, HealthcareProfessional professional, MedicalService service, DateTime start)
    {
        var end = start + service.Duration;

        if (professional.Branch == null)
            throw new InvalidOperationException("Sağlık personeli herhangi bir şubeye atanmamış.");

        if (!professional.Branch.IsOpenBetween(start, end))
            throw new AppointmentConflictException("Şube seçilen saatte kapalı.");

        if (!professional.IsAvailable(start, end))
            throw new AppointmentConflictException("Sağlık personeli seçilen saatte müsait değil.");

        if (_repository.GetActiveByProfessional(professional.Id).Any(a => a.Overlaps(start, end)))
            throw new AppointmentConflictException("Sağlık personelinin bu saatte başka bir randevusu var.");

        if (_repository.GetActiveByPatient(patient.Id).Any(a => a.Overlaps(start, end)))
            throw new AppointmentConflictException("Bu saatte başka bir randevunuz zaten var.");

        var appointment = new Appointment(patient, professional, service, start);
        _repository.Add(appointment);
        return appointment;
    }

    public void CancelByPatient(int appointmentId, int patientId)
    {
        var appointment = GetOrThrow(appointmentId);
        if (appointment.PatientId != patientId)
            throw new InvalidOperationException("Yalnızca kendi randevularınızı iptal edebilirsiniz.");

        appointment.Cancel();
        _repository.Update(appointment);
    }

    public void ApproveByProfessional(int appointmentId, int professionalId)
    {
        var appointment = GetForProfessionalOrThrow(appointmentId, professionalId);
        appointment.Approve();
        _repository.Update(appointment);
    }

    public void RejectByProfessional(int appointmentId, int professionalId, string reason)
    {
        var appointment = GetForProfessionalOrThrow(appointmentId, professionalId);
        appointment.Reject(reason);
        _repository.Update(appointment);
    }

    public void CompleteByProfessional(int appointmentId, int professionalId)
    {
        var appointment = GetForProfessionalOrThrow(appointmentId, professionalId);
        appointment.Complete();
        _repository.Update(appointment);
    }

    private Appointment GetForProfessionalOrThrow(int appointmentId, int professionalId)
    {
        var appointment = GetOrThrow(appointmentId);
        if (appointment.ProfessionalId != professionalId)
            throw new InvalidOperationException("Bu randevu size ait değil.");
        return appointment;
    }

    private Appointment GetOrThrow(int appointmentId)
    {
        return _repository.GetById(appointmentId)
            ?? throw new KeyNotFoundException("Randevu bulunamadı.");
    }
}