using System;
using CheckoutPricing.Api.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace CheckoutPricing.Api.Tests.StepDefinitions
{
    [Binding]
    public class CheckoutStepDefinitions
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response;

        public CheckoutStepDefinitions()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Given(@"the following pricing rules:")]
        public async Task GivenTheFollowingPricingRules(Table table)
        {
            var pricingRules = new List<PricingRule>();

            foreach (var row in table.Rows)
            {
                pricingRules.Add(new PricingRule
                {
                    Item = row["Item"],
                    UnitPrice = decimal.Parse(row["UnitPrice"]),
                    SpecialQuantity = row.ContainsKey("SpecialQuantity") && int.TryParse(row["SpecialQuantity"], out var sq) ? sq : (int?)null,
                    SpecialPrice = row.ContainsKey("SpecialPrice") && decimal.TryParse(row["SpecialPrice"], out var sp) ? sp : (decimal?)null
                });
            }

            var content = new StringContent(JsonConvert.SerializeObject(pricingRules), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync("/checkout/pricing/rules", content);
            _response.EnsureSuccessStatusCode();
        }

        [When(@"I scan the following items:")]
        public async Task WhenIScanTheFollowingItems(Table table)
        {
            foreach (var row in table.Rows)
            {
                _response = await _client.PostAsync($"/checkout/scan/{row["Item"]}", null);
                _response.EnsureSuccessStatusCode();
            }
        }

        [When(@"I scan the item ""(.*)""")]
        public async Task WhenIScanTheItem(string item)
        {
            _response = await _client.PostAsync($"/checkout/scan/{item}", null);
            _response.EnsureSuccessStatusCode();
        }

        [Then(@"the total price should be (.*)")]
        public async Task ThenTheTotalPriceShouldBe(int expectedTotal)
        {
            _response = await _client.GetAsync("/checkout/total");
            _response.EnsureSuccessStatusCode();

            var responseContent = await _response.Content.ReadAsStringAsync();
            var actualTotal = decimal.Parse(responseContent);

            Assert.Equal(expectedTotal, actualTotal);
        }
    }
}