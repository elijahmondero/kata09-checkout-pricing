namespace CheckoutPricing.Api.Models;

public class PricingRule
{
    public string Item { get; set; }
    public decimal UnitPrice { get; set; }
    public int? SpecialQuantity { get; set; }
    public decimal? SpecialPrice { get; set; }
}