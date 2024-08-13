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
public class ProductController(IOptions<DatabaseSettings> databaseSettings, ILogger<ProductController> logger)
    : ControllerBase
{
    private readonly string _connectionString = databaseSettings.Value.ConnectionString!;

    private QueryFactory CreateQueryFactory()
    {
        var connection = new MySqlConnection(_connectionString);
        var compiler = new MySqlCompiler();
        return new QueryFactory(connection, compiler);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        try
        {
            var db = CreateQueryFactory();
            await db.Query("Products").InsertAsync(new { product.Id, product.Name, product.UnitPrice });
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding product");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product updatedProduct)
    {
        try
        {
            var db = CreateQueryFactory();
            var affectedRows = await db.Query("Products").Where("Id", id).UpdateAsync(new { updatedProduct.Name, updatedProduct.UnitPrice });

            if (affectedRows == 0)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveProduct(string id)
    {
        try
        {
            var db = CreateQueryFactory();
            var affectedRows = await db.Query("Products").Where("Id", id).DeleteAsync();

            if (affectedRows == 0)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            const string query = "SELECT * FROM Products WHERE Id = @Id";
            var product = await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving product");
            return StatusCode(500, "Internal server error");
        }
    }
}
