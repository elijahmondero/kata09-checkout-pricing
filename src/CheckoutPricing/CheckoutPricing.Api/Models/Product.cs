﻿namespace CheckoutPricing.Api.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}
