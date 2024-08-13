namespace CheckoutPricing.Api.Models;

public class PricingRule
{
    public string ProductId { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int? SpecialQuantity { get; set; }
    public decimal? SpecialPrice { get; set; }
}