using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Services;

public class AvailabilityService
{
    private readonly AppDbContext _context;

    public AvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    public void AddSlot(int doctorId, DateTime start, DateTime end)
    {
        if (start <= DateTime.Now)
            throw new ArgumentException("Availability must be in the future.");

        var doctor = _context.Doctors
            .Include(d => d.Branch).ThenInclude(b => b!.Schedule)
            .Include(d => d.Availability)
            .FirstOrDefault(d => d.Id == doctorId)
            ?? throw new KeyNotFoundException("Doctor not found.");

        doctor.AddAvailability(new AvailabilitySlot(start, end));
        _context.SaveChanges();
    }

    public void RemoveSlot(int slotId, int doctorId)
    {
        var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.Id == slotId && s.DoctorId == doctorId)
            ?? throw new KeyNotFoundException("Slot not found.");

        var hasActiveAppointments = _context.Appointments.Any(a =>
            a.DoctorId == doctorId
            && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved)
            && a.StartTime < slot.End && slot.Start < a.EndTime);

        if (hasActiveAppointments)
            throw new InvalidOperationException("This slot has active appointments and cannot be removed.");

        _context.AvailabilitySlots.Remove(slot);
        _context.SaveChanges();
    }
}