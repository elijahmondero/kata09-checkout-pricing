using DbUp;
using System.Reflection;

namespace CheckoutPricing.Api.Data;

public class DatabaseMigrator
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(string connectionString, ILogger<DatabaseMigrator> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void MigrateDatabase()
    {
        var upgrader = DeployChanges.To
            .MySqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToAutodetectedLog()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Database migration failed");
            throw new Exception("Database migration failed", result.Error);
        }

        _logger.LogInformation("Database migration succeeded");
    }
}

