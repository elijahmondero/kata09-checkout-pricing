using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MySql;

namespace CheckoutPricing.Api.Tests.Support.Data;

public class MySqlContainerFixture : IAsyncLifetime
{
    public MySqlContainer MySqlContainer { get; private set; }

    public MySqlContainerFixture()
    {
        var mysqlImageName = Environment.GetEnvironmentVariable("MYSQL_TEST_VERSION") ?? "mysql:8.0";
        MySqlContainer = new MySqlBuilder()
            .WithImage(mysqlImageName)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await MySqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await MySqlContainer.StopAsync();
    }

}

[CollectionDefinition("MySqlContainer")]
public class MySqlContainerCollection : ICollectionFixture<MySqlContainerFixture>
{
}