using CheckoutPricing.Api.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Data;
using CheckoutPricing.Api.Tests.Support.Data;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace CheckoutPricing.Api.Tests.StepDefinitions;

[Binding]
[Collection("MySqlContainer")]
public class CheckoutStepDefinitions
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private readonly MySqlContainerFixture _fixture;

    public CheckoutStepDefinitions(MySqlContainerFixture fixture)
    {
        _fixture = fixture;
        //_fixture.StartContainer();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var settings = new Dictionary<string, string>
                    {
                        { "DatabaseSettings:ConnectionString", _fixture.MySqlContainer.GetConnectionString() }
                    };
                    config.AddInMemoryCollection(settings!);
                });
            });
        _client = factory.CreateClient();
    }


    [BeforeFeature]
    public static async Task Before(MySqlContainerFixture fixture)
    {
        await fixture.InitializeAsync();
    }

    [AfterFeature]
    public static async Task After(MySqlContainerFixture fixture)
    {
        await fixture.DisposeAsync();
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
                SpecialQuantity = row.ContainsKey("SpecialQuantity") && int.TryParse(row["SpecialQuantity"], out var sq)
                    ? sq
                    : (int?)null,
                SpecialPrice = row.ContainsKey("SpecialPrice") && decimal.TryParse(row["SpecialPrice"], out var sp)
                    ? sp
                    : (decimal?)null
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
