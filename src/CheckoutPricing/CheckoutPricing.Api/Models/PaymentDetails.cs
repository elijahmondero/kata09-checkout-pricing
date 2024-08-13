namespace CheckoutPricing.Api.Models;

public class PaymentDetails
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string CardExpiry { get; set; } = string.Empty;
    public string CardCvc { get; set; } = string.Empty;
}