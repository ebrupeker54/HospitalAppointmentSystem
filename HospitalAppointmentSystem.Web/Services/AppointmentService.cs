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

    public List<DateTime> GetAvailableStartTimes(Doctor doctor, MedicalService service, DateTime date)
    {
        var result = new List<DateTime>();
        if (doctor.Branch == null || !doctor.CanPerform(service))
            return result;

        var booked = _repository.GetActiveByDoctor(doctor.Id).ToList();

        foreach (var slot in doctor.Availability.Where(s => s.Start.Date == date.Date).OrderBy(s => s.Start))
        {
            for (var start = slot.Start; start + service.Duration <= slot.End; start += service.Duration)
            {
                var end = start + service.Duration;

                if (start <= DateTime.Now) continue;
                if (!doctor.Branch.IsOpenBetween(start, end)) continue;
                if (booked.Any(a => a.Overlaps(start, end))) continue;

                result.Add(start);
            }
        }

        return result;
    }

    public Appointment Book(Patient patient, Doctor doctor, MedicalService service, DateTime start)
    {
        var end = start + service.Duration;

        if (doctor.Branch == null)
            throw new InvalidOperationException("The doctor is not assigned to a branch.");

        if (!doctor.Branch.IsOpenBetween(start, end))
            throw new AppointmentConflictException("The branch is closed at the selected time.");

        if (!doctor.IsAvailable(start, end))
            throw new AppointmentConflictException("The doctor is not available at the selected time.");

        if (_repository.GetActiveByDoctor(doctor.Id).Any(a => a.Overlaps(start, end)))
            throw new AppointmentConflictException("The doctor already has an appointment at this time.");

        if (_repository.GetActiveByPatient(patient.Id).Any(a => a.Overlaps(start, end)))
            throw new AppointmentConflictException("You already have another appointment at this time.");

        var appointment = new Appointment(patient, doctor, service, start);
        _repository.Add(appointment);
        return appointment;
    }

    public void Approve(int appointmentId)
    {
        var appointment = GetOrThrow(appointmentId);
        appointment.Approve();
        _repository.Update(appointment);
    }

    public void Reject(int appointmentId, string reason)
    {
        var appointment = GetOrThrow(appointmentId);
        appointment.Reject(reason);
        _repository.Update(appointment);
    }

    public void Cancel(int appointmentId)
    {
        var appointment = GetOrThrow(appointmentId);
        appointment.Cancel();
        _repository.Update(appointment);
    }

    public void CancelByPatient(int appointmentId, int patientId)
    {
        var appointment = GetOrThrow(appointmentId);
        if (appointment.PatientId != patientId)
            throw new InvalidOperationException("You can only cancel your own appointments.");

        appointment.Cancel();
        _repository.Update(appointment);
    }

    private Appointment GetOrThrow(int appointmentId)
    {
        return _repository.GetById(appointmentId)
            ?? throw new KeyNotFoundException($"Appointment {appointmentId} was not found.");
    }
}