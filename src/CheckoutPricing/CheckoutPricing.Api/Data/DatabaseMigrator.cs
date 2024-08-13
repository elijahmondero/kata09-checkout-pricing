using DbUp;
using System.Reflection;

namespace CheckoutPricing.Api.Data;

public class DatabaseMigrator(string connectionString, ILogger<DatabaseMigrator> logger)
{
    public void MigrateDatabase()
    {
        // Ensure the database is created if it does not exist
        EnsureDatabase.For.MySqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .MySqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToAutodetectedLog()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Database migration failed");
            throw new Exception("Database migration failed", result.Error);
        }

        logger.LogInformation("Database migration succeeded");
    }
}

