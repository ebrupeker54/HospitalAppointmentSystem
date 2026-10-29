namespace HospitalAppointmentSystem.Web.Models;

public class Payment
{
    public int Id { get; private set; }

    public int AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }

    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string TransactionReference { get; private set; } = string.Empty;
    public string? RefundReference { get; private set; }
    public DateTime PaidAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }

    // Required by Entity Framework Core
    private Payment() { }

    public Payment(Appointment appointment, string provider, string transactionReference)
    {
        if (!appointment.CanBePaid)
            throw new InvalidOperationException("Bu randevu için ödeme yapılamaz.");

        Appointment = appointment;
        AppointmentId = appointment.Id;
        Amount = appointment.Price;
        Status = PaymentStatus.Paid;
        Provider = provider;
        TransactionReference = transactionReference;
        PaidAt = DateTime.Now;
    }

    public void MarkRefunded(string refundReference)
    {
        if (Status != PaymentStatus.Paid)
            throw new InvalidOperationException("Yalnızca tamamlanmış ödemeler iade edilebilir.");

        Status = PaymentStatus.Refunded;
        RefundReference = refundReference;
        RefundedAt = DateTime.Now;
    }
}