namespace HospitalAppointmentSystem.Web.Services;

public record PaymentResult(bool Succeeded, string Reference, string? ErrorMessage = null);

public interface IPaymentProvider
{
    string Name { get; }

    PaymentResult Charge(decimal amount, string description);

    PaymentResult Refund(string transactionReference, decimal amount);
}