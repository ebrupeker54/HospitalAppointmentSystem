using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Services;

public class PaymentService
{
    private readonly AppDbContext _context;
    private readonly IPaymentProvider _provider;

    public PaymentService(AppDbContext context, IPaymentProvider provider)
    {
        _context = context;
        _provider = provider;
    }

    public Payment PayForAppointment(int appointmentId, int patientId)
    {
        var appointment = _context.Appointments
            .Include(a => a.Payment)
            .FirstOrDefault(a => a.Id == appointmentId)
            ?? throw new KeyNotFoundException("Randevu bulunamadı.");

        if (appointment.PatientId != patientId)
            throw new InvalidOperationException("Yalnızca kendi randevularınız için ödeme yapabilirsiniz.");

        if (!appointment.CanBePaid)
            throw new InvalidOperationException("Bu randevu için ödeme yapılamaz.");

        var result = _provider.Charge(appointment.Price, $"Randevu #{appointment.Id}");
        if (!result.Succeeded)
            throw new InvalidOperationException(result.ErrorMessage ?? "Ödeme alınamadı.");

        var payment = new Payment(appointment, _provider.Name, result.Reference);
        _context.Payments.Add(payment);
        _context.SaveChanges();
        return payment;
    }
}