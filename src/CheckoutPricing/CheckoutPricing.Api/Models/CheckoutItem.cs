namespace CheckoutPricing.Api.Models;

public class CheckoutItem
{
    public string SessionId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}