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

    public void AddSlot(int professionalId, DateTime start, DateTime end)
    {
        if (start <= DateTime.Now)
            throw new ArgumentException("Uygunluk aralığı ileri bir tarih için olmalıdır.");

        var professional = _context.HealthcareProfessionals
            .Include(p => p.Branch).ThenInclude(b => b!.Schedule)
            .Include(p => p.Availability)
            .FirstOrDefault(p => p.Id == professionalId)
            ?? throw new KeyNotFoundException("Sağlık personeli bulunamadı.");

        professional.AddAvailability(new AvailabilitySlot(start, end));
        _context.SaveChanges();
    }

    public void RemoveSlot(int slotId, int professionalId)
    {
        var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.Id == slotId && s.ProfessionalId == professionalId)
            ?? throw new KeyNotFoundException("Uygunluk aralığı bulunamadı.");

        var hasActiveAppointments = _context.Appointments.Any(a =>
            a.ProfessionalId == professionalId
            && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved)
            && a.StartTime < slot.End && slot.Start < a.EndTime);

        if (hasActiveAppointments)
            throw new InvalidOperationException("Bu aralıkta aktif randevular olduğu için silinemez.");

        _context.AvailabilitySlots.Remove(slot);
        _context.SaveChanges();
    }
}