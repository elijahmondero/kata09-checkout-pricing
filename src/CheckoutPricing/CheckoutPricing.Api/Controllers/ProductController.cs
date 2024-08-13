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

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
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

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="updatedProduct">The updated product details.</param>
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

    /// <summary>
    /// Removes a product.
    /// </summary>
    /// <param name="id">The ID of the product to remove.</param>
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

    /// <summary>
    /// Retrieves a product by ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
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

    /// <summary>
    /// Retrieves a paged list of products with optional search.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="search">The search term for product names.</param>
    [HttpGet("")]
    public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, string? search = null)
    {
        try
        {
            var db = CreateQueryFactory();
            var query = db.Query("Products");

            if (!string.IsNullOrEmpty(search))
            {
                query = query.WhereLike("Name", $"%{search}%");
            }

            var products = await query
                .ForPage(page, pageSize)
                .GetAsync<Product>();

            return Ok(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "Internal server error");
        }
    }
}