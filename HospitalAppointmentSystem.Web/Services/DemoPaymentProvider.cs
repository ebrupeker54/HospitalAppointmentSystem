namespace HospitalAppointmentSystem.Web.Services;

// Simulates a payment gateway: no money is charged and no card data is received.
public class DemoPaymentProvider : IPaymentProvider
{
    public string Name => "Demo";

    public PaymentResult Charge(decimal amount, string description) =>
        new(true, NewReference("DEMO"));

    public PaymentResult Refund(string transactionReference, decimal amount) =>
        new(true, NewReference("REFUND"));

    private static string NewReference(string prefix) =>
        $"{prefix}-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}";
}