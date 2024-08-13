namespace CheckoutPricing.Api.Models;

public class CheckoutSession
{
    public string SessionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public decimal? TotalAmount { get; set; }
}
