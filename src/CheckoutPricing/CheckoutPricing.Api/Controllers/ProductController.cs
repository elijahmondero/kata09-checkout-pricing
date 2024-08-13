using CheckoutPricing.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPricing.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private static readonly List<Product> Products = new();

    [HttpPost("")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        Products.Add(product);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(string id, [FromBody] Product updatedProduct)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = updatedProduct.Name;
        product.UnitPrice = updatedProduct.UnitPrice;
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveProduct(string id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        Products.Remove(product);
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(string id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}
