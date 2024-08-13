using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using CheckoutPricing.Api.Models;
using CheckoutPricing.Api.Tests.Support.Data;
using CheckoutPricing.Api.Tests.Support;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using TechTalk.SpecFlow;

namespace CheckoutPricing.Api.Tests.StepDefinitions;

[Binding]
[Collection("MySqlContainer")]
public class ProductStepDefinitions
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private readonly ITestOutputHelper _output;
    private readonly string _connectionString;
    
    public ProductStepDefinitions(MySqlContainerFixture fixture, ITestOutputHelper output)
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
        _output = output;
        _connectionString = fixture.MySqlContainer.GetConnectionString();
    }

    private void PurgeDatabase()
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        using var command = new MySqlCommand("DELETE FROM Products", connection);
        command.ExecuteNonQuery();
    }

    [Given(@"I add the following product:")]
    public async Task GivenIAddTheFollowingProduct(Table table)
    {
        foreach (var row in table.Rows)
        {
            var originalId = row["Id"];
            var originalName = row["Name"];
            var product = new Product
            {
                Id = originalId,
                Name = originalName,
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

    [When(@"I request page (.*) with page size (.*)")]
    public async Task WhenIRequestPageWithPageSize(int page, int pageSize)
    {
        _response = await _client.GetAsync($"/Product?page={page}&pageSize={pageSize}");
        _response.EnsureSuccessStatusCode();
    }

    [Then(@"the response should contain the following products:")]
    public async Task ThenTheResponseShouldContainTheFollowingProducts(Table table)
    {
        var responseContent = await _response!.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<List<Product>>(responseContent);
        _output.WriteLine(responseContent);
        foreach (var row in table.Rows)
        {
            var product = products!.FirstOrDefault(p => p.Id == row["Id"]);
            Assert.NotNull(product);
            Assert.Equal(row["Name"], product!.Name);
            Assert.Equal(decimal.Parse(row["UnitPrice"]), product.UnitPrice);
        }
    }

    [When(@"I search for products with name containing ""(.*)""")]
    public async Task WhenISearchForProductsWithNameContaining(string search)
    {
        _response = await _client.GetAsync($"/Product?search={search}");
        _response.EnsureSuccessStatusCode();
    }
}