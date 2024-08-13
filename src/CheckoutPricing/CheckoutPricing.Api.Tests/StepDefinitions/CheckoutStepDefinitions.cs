using CheckoutPricing.Api.Models;
using System.Text;
using CheckoutPricing.Api.Tests.Support;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using CheckoutPricing.Api.Tests.Support.Data;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CheckoutPricing.Api.Tests.StepDefinitions;

[Binding]
[Collection("MySqlContainer")]
public class CheckoutStepDefinitions
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private string? _sessionId;

    public CheckoutStepDefinitions(MySqlContainerFixture fixture, ITestOutputHelper output)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var settings = new Dictionary<string, string>
                    {
                        { "DatabaseSettings:ConnectionString", fixture.MySqlContainer.GetConnectionString() }
                    };
                    config.AddInMemoryCollection(settings!);
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(new TestOutputLoggerProvider(output));
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

    [Given(@"the following products exist:")]
    public async Task GivenTheFollowingProductsExist(Table table)
    {
        foreach (var row in table.Rows)
        {
            var product = new Product
            {
                Id = row["Id"],
                Name = row["Name"],
                UnitPrice = decimal.Parse(row["UnitPrice"])
            };

            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync("/product", content);
            _response.EnsureSuccessStatusCode();
        }
    }

    [Given(@"the following pricing rules:")]
    public async Task GivenTheFollowingPricingRules(Table table)
    {
        var pricingRules = new List<PricingRule>();

        foreach (var row in table.Rows)
        {
            pricingRules.Add(new PricingRule
            {
                ProductId = row["Item"],
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

    [When(@"I start a new checkout session")]
    public async Task WhenIStartANewCheckoutSession()
    {
        _response = await _client.PostAsync("/checkout/session/start", null);
        _response.EnsureSuccessStatusCode();

        var responseContent = await _response.Content.ReadAsStringAsync();
        var sessionData = JsonConvert.DeserializeObject<CheckoutSession>(responseContent);
        _sessionId = sessionData!.SessionId;
    }

    [When(@"I scan the following items:")]
    public async Task WhenIScanTheFollowingItems(Table table)
    {
        foreach (var row in table.Rows)
        {
            _response = await _client.PostAsync($"/checkout/scan/{_sessionId}/{row["Item"]}", null);
            _response.EnsureSuccessStatusCode();
        }
    }

    [When(@"I scan the item ""(.*)""")]
    public async Task WhenIScanTheItem(string item)
    {
        _response = await _client.PostAsync($"/checkout/scan/{_sessionId}/{item}", null);
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"the total price should be (.*)")]
    public async Task ThenTheTotalPriceShouldBe(decimal expectedTotal)
    {
        _response = await _client.GetAsync($"/checkout/total/{_sessionId}");
        _response.EnsureSuccessStatusCode();

        var responseContent = await _response.Content.ReadAsStringAsync();
        var actualTotal = decimal.Parse(responseContent);

        Assert.Equal(expectedTotal, actualTotal);
    }

    [When(@"I end the checkout session with payment details:")]
    public async Task WhenIEndTheCheckoutSessionWithPaymentDetails(Table table)
    {
        var paymentDetails = new PaymentDetails
        {
            PaymentMethod = table.Rows[0]["PaymentMethod"],
            CardNumber = table.Rows[0]["CardNumber"],
            CardExpiry = table.Rows[0]["CardExpiry"],
            CardCvc = table.Rows[0]["CardCvc"]
        };

        var content = new StringContent(JsonConvert.SerializeObject(paymentDetails), Encoding.UTF8, "application/json");
        _response = await _client.PostAsync($"/checkout/session/end/{_sessionId}", content);
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"I end the checkout session with payment details:")]
    public async Task ThenIEndTheCheckoutSessionWithPaymentDetails(Table table)
    {
        var paymentDetails = new PaymentDetails
        {
            PaymentMethod = table.Rows[0]["PaymentMethod"],
            CardNumber = table.Rows[0]["CardNumber"],
            CardExpiry = table.Rows[0]["CardExpiry"],
            CardCvc = table.Rows[0]["CardCvc"]
        };

        var content = new StringContent(JsonConvert.SerializeObject(paymentDetails), Encoding.UTF8, "application/json");
        _response = await _client.PostAsync($"/checkout/session/end/{_sessionId}", content);
        _response.EnsureSuccessStatusCode();
    }
}