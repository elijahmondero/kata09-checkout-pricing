using CheckoutPricing.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPricing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : ControllerBase
    {
        private static readonly List<PricingRule> PricingRules = [];
        private static readonly Dictionary<string, int> Items = new();


        [HttpPost("pricing/rules")]
        public IActionResult SetPricingRules([FromBody] List<PricingRule> rules)
        {
            PricingRules.Clear();
            PricingRules.AddRange(rules);
            Items.Clear(); // Clear items on pricing rule change
            return Ok();
        }

        [HttpPost("scan/{item}")]
        public IActionResult ScanItem(string item)
        {
            Items.TryAdd(item, 0);
            Items[item]++;
            return Ok();
        }

        [HttpGet("total")]
        public IActionResult GetTotal()
        {
            decimal total = 0;

            foreach (var item in Items)
            {
                var rule = PricingRules.Find(r => r.Item == item.Key);
                if (rule == null) continue;
                if (rule.SpecialQuantity.HasValue && item.Value >= rule.SpecialQuantity)
                {
                    var specialBundleCount = item.Value / rule.SpecialQuantity.Value;
                    var remainingItems = item.Value % rule.SpecialQuantity.Value;
                    if (rule.SpecialPrice != null)
                        total += specialBundleCount * rule.SpecialPrice.Value + remainingItems * rule.UnitPrice;
                }
                else
                {
                    total += item.Value * rule.UnitPrice;
                }
            }

            return Ok(total);
        }
    }
}
