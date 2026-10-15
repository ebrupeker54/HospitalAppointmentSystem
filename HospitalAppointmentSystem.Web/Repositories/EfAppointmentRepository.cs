using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Repositories;

public class EfAppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public EfAppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Appointment? GetById(int id)
    {
        return _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.MedicalService)
            .FirstOrDefault(a => a.Id == id);
    }

    public IEnumerable<Appointment> GetActiveByDoctor(int doctorId)
    {
        return _context.Appointments
            .Where(a => a.DoctorId == doctorId
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved))
            .ToList();
    }

    public IEnumerable<Appointment> GetActiveByPatient(int patientId)
    {
        return _context.Appointments
            .Where(a => a.PatientId == patientId
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved))
            .ToList();
    }

    public void Add(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        _context.SaveChanges();
    }

    public void Update(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        _context.SaveChanges();
    }
}