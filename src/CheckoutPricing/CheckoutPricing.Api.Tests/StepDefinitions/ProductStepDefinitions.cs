using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using CheckoutPricing.Api.Models;
using CheckoutPricing.Api.Tests.Support.Data;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace CheckoutPricing.Api.Tests.StepDefinitions;

[Binding]
[Collection("MySqlContainer")]
public class ProductStepDefinitions
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private readonly MySqlContainerFixture _fixture;

    public ProductStepDefinitions(MySqlContainerFixture fixture)
    {
        _fixture = fixture;

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

    [Given(@"I add the following product:")]
    public async Task GivenIAddTheFollowingProduct(Table table)
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
            _response = await _client.PostAsync("/Product", content);
            _response.EnsureSuccessStatusCode();
        }
    }

    [When(@"I update the product with:")]
    public async Task WhenIUpdateTheProductWith(Table table)
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
            _response = await _client.PutAsync($"/Product/{product.Id}", content);
            _response.EnsureSuccessStatusCode();
        }
    }

    [When(@"I remove the product with id ""(.*)""")]
    public async Task WhenIRemoveTheProductWithId(string productId)
    {
        _response = await _client.DeleteAsync($"/Product/{productId}");
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"the product ""(.*)"" should exist")]
    public async Task ThenTheProductShouldExist(string productId)
    {
        _response = await _client.GetAsync($"/Product/{productId}");
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"the product ""(.*)"" should have the name ""(.*)"" and the unit price (.*)")]
    public async Task ThenTheProductShouldHaveTheNameAndTheUnitPrice(string productId, string name, decimal unitPrice)
    {
        _response = await _client.GetAsync($"/Product/{productId}");
        _response.EnsureSuccessStatusCode();

        var product = JsonConvert.DeserializeObject<Product>(await _response.Content.ReadAsStringAsync());
        if (product!.Name != name || product.UnitPrice != unitPrice)
        {
            throw new Exception("Product details do not match.");
        }
    }

    [Then(@"the product ""(.*)"" should not exist")]
    public async Task ThenTheProductShouldNotExist(string productId)
    {
        _response = await _client.GetAsync($"/Product/{productId}");
        if (_response.IsSuccessStatusCode)
        {
            throw new Exception("Product still exists.");
        }
    }
}
