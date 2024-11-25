using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Add Cosmos DB client
        services.AddSingleton(sp =>
        {
            var connectionString = context.Configuration["CosmosDbConnection"];
            return new CosmosClient(connectionString);
        });

        // Add Cosmos DB container
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var database = client.GetDatabase(context.Configuration["CosmosDbDatabaseName"]);
            return database.GetContainer(context.Configuration["CosmosDbContainerName"]);
        });
    })
    .Build();

host.Run();