using CheckoutPricing.Api.Data;
using CheckoutPricing.Api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace CheckoutPricing.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckoutController(IOptions<DatabaseSettings> databaseSettings, ILogger<CheckoutController> logger)
    : ControllerBase
{
    private readonly string _connectionString = databaseSettings.Value.ConnectionString!;

    private QueryFactory CreateQueryFactory()
    {
        var connection = new MySqlConnection(_connectionString);
        var compiler = new MySqlCompiler();
        return new QueryFactory(connection, compiler);
    }

    /// <summary>
    /// Starts a new checkout session.
    /// </summary>
    [HttpPost("session/start")]
    public async Task<IActionResult> StartSession()
    {
        try
        {
            var db = CreateQueryFactory();
            var sessionId = Guid.NewGuid().ToString();
            await db.Query("CheckoutSessions").InsertAsync(new { SessionId = sessionId, Status = "Active", CreatedAt = DateTime.UtcNow });

            return Ok(new { SessionId = sessionId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting checkout session");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Ends an active checkout session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to end.</param>
    /// <param name="paymentDetails">The payment details for the session.</param>
    [HttpPost("session/end/{sessionId}")]
    public async Task<IActionResult> EndSession(string sessionId, [FromBody] PaymentDetails paymentDetails)
    {
        try
        {
            var db = CreateQueryFactory();
            var session = await db.Query("CheckoutSessions").Where("SessionId", sessionId).FirstOrDefaultAsync();

            if (session == null || session!.Status != "Active")
            {
                return BadRequest("Invalid or inactive session");
            }

            var total = await CalculateTotal(sessionId);
            // Process payment (this is a placeholder, actual payment processing logic should be implemented)
            var paymentSuccess = ProcessPayment(paymentDetails, total);

            if (paymentSuccess)
            {
                await db.Query("CheckoutSessions").Where("SessionId", sessionId).UpdateAsync(new { Status = "Completed", TotalAmount = total, EndedAt = DateTime.UtcNow });
                return Ok(new { TotalAmount = total });
            }
            else
            {
                return StatusCode(500, "Payment processing failed");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error ending checkout session");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Cancels an active checkout session.
    /// </summary>
    /// <param name="sessionId">The ID of the session to cancel.</param>
    [HttpPost("session/cancel/{sessionId}")]
    public async Task<IActionResult> CancelSession(string sessionId)
    {
        try
        {
            var db = CreateQueryFactory();
            var session = await db.Query("CheckoutSessions").Where("SessionId", sessionId).FirstOrDefaultAsync();

            if (session == null || session!.Status != "Active")
            {
                return BadRequest("Invalid or inactive session");
            }

            await db.Query("CheckoutSessions").Where("SessionId", sessionId).UpdateAsync(new { Status = "Cancelled", EndedAt = DateTime.UtcNow });
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling checkout session");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Scans a product and adds it to the checkout session.
    /// </summary>
    /// <param name="sessionId">The ID of the session.</param>
    /// <param name="productId">The ID of the product to scan.</param>
    [HttpPost("scan/{sessionId}/{productId}")]
    public async Task<IActionResult> ScanProduct(string sessionId, string productId)
    {
        try
        {
            var db = CreateQueryFactory();
            var session = await db.Query("CheckoutSessions").Where("SessionId", sessionId).FirstOrDefaultAsync();

            if (session == null || session!.Status != "Active")
            {
                return BadRequest("Invalid or inactive session");
            }

            var existingItem = await db.Query("CheckoutItems").Where("SessionId", sessionId).Where("ProductId", productId).FirstOrDefaultAsync();

            if (existingItem == null)
            {
                await db.Query("CheckoutItems").InsertAsync(new { SessionId = sessionId, ProductId = productId, Quantity = 1 });
            }
            else
            {
                await db.Query("CheckoutItems").Where("SessionId", sessionId).Where("ProductId", productId).IncrementAsync("Quantity", 1);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error scanning item {ex}");
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Gets the total amount for the checkout session.
    /// </summary>
    /// <param name="sessionId">The ID of the session.</param>
    [HttpGet("total/{sessionId}")]
    public async Task<IActionResult> GetTotal(string sessionId)
    {
        try
        {
            var total = await CalculateTotal(sessionId);
            return Ok(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating total");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Sets the pricing rules for products.
    /// </summary>
    /// <param name="pricingRules">The list of pricing rules to set.</param>
    [HttpPost("pricing/rules")]
    public async Task<IActionResult> SetPricingRules([FromBody] List<PricingRule> pricingRules)
    {
        try
        {
            var db = CreateQueryFactory();
            foreach (var rule in pricingRules)
            {
                var existingRule = await db.Query("PricingRules").Where("ProductId", rule.ProductId).FirstOrDefaultAsync();
                if (existingRule == null)
                {
                    await db.Query("PricingRules").InsertAsync(rule);
                }
                else
                {
                    await db.Query("PricingRules").Where("ProductId", rule.ProductId).UpdateAsync(rule);
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting pricing rules");
            return StatusCode(500, "Internal server error");
        }
    }

    private async Task<decimal> CalculateTotal(string sessionId)
    {
        var db = CreateQueryFactory();
        var items = await db.Query("CheckoutItems").Where("SessionId", sessionId).GetAsync<CheckoutItem>();
        var pricingRules = await db.Query("PricingRules").GetAsync<PricingRule>();


            
        decimal total = 0;

        foreach (var item in items)
        {
            var rule = pricingRules.FirstOrDefault(r => r.ProductId == item.ProductId);
            if (rule == null) continue;

            if (rule.SpecialQuantity.HasValue && item.Quantity >= rule.SpecialQuantity)
            {
                var specialBundleCount = item.Quantity / rule.SpecialQuantity!.Value;
                var remainingItems = item.Quantity % rule.SpecialQuantity.Value;
                var product = await db.Query("Products").Where("Id", item.ProductId).FirstOrDefaultAsync();
                if (product == null) throw new InvalidOperationException("Product does not exist.");
                if (rule.SpecialPrice != null)
                    total += specialBundleCount * rule.SpecialPrice.Value + remainingItems * product.UnitPrice;
            }
            else
            {
                total += item.Quantity * rule.UnitPrice;
            }
        }

        return total;
    }

    private bool ProcessPayment(PaymentDetails paymentDetails, decimal total)
    {
        // Placeholder for payment processing logic
        return true;
    }
}