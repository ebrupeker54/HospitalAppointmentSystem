using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Services;

public class ResultService
{
    private readonly AppDbContext _context;

    public ResultService(AppDbContext context)
    {
        _context = context;
    }

    public void EnterResult(int appointmentId, int professionalId, string? findings)
    {
        var appointment = _context.Appointments
            .Include(a => a.Professional)
            .Include(a => a.Result)
            .FirstOrDefault(a => a.Id == appointmentId)
            ?? throw new KeyNotFoundException("Randevu bulunamadı.");

        if (appointment.ProfessionalId != professionalId)
            throw new InvalidOperationException("Bu randevu size ait değil.");

        if (appointment.Professional is not IResultEntry writer)
            throw new InvalidOperationException("Sonuç girme yetkiniz bulunmuyor.");

        if (appointment.Result != null)
            throw new InvalidOperationException("Bu randevu için sonuç zaten girilmiş.");

        var result = writer.WriteResult(appointment, findings ?? string.Empty);
        _context.TestResults.Add(result);
        _context.SaveChanges();
    }
}