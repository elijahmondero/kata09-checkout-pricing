namespace CheckoutPricing.Api.Models;

public class Product
{
    public string Id { get; set; } // Unique identifier, e.g., SKU
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
}
