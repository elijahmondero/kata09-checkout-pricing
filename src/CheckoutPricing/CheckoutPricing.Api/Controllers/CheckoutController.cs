using CheckoutPricing.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPricing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : ControllerBase
    {
        private static readonly List<PricingRule> PricingRules = new List<PricingRule>();
        private static readonly Dictionary<string, int> Items = new Dictionary<string, int>();


        [HttpPost("pricingrules")]
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
            if (!Items.ContainsKey(item))
            {
                Items[item] = 0;
            }
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
                if (rule != null)
                {
                    if (rule.SpecialQuantity.HasValue && item.Value >= rule.SpecialQuantity)
                    {
                        int specialBundleCount = item.Value / rule.SpecialQuantity.Value;
                        int remainingItems = item.Value % rule.SpecialQuantity.Value;
                        total += specialBundleCount * rule.SpecialPrice.Value + remainingItems * rule.UnitPrice;
                    }
                    else
                    {
                        total += item.Value * rule.UnitPrice;
                    }
                }
            }

            return Ok(total);
        }
    }
}
