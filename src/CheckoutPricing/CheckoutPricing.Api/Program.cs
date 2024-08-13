using CheckoutPricing.Api.Data;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Bind DatabaseSettings from configuration
        builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

        // Register DatabaseMigrator
        builder.Services.AddSingleton<DatabaseMigrator>(sp =>
        {
            var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            if (string.IsNullOrWhiteSpace(databaseSettings.ConnectionString))
            {
                throw new InvalidOperationException("Database connection string is missing");
            }
            var logger = sp.GetRequiredService<ILogger<DatabaseMigrator>>();
            return new DatabaseMigrator(databaseSettings.ConnectionString, logger);
        });

        var app = builder.Build();

        // Perform database migration
        using (var scope = app.Services.CreateScope())
        {
            var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
            migrator.MigrateDatabase();
        }

        // Configure the HTTP request pipeline.

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}