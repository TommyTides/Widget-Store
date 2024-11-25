namespace WidgetStore.Core.Configuration;

/// <summary>
/// Configuration for Cosmos DB
/// </summary>
public class CosmosDbConfig
{
    /// <summary>
    /// Gets or sets the connection string
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the database name
    /// </summary>
    public string DatabaseName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the container name for storing all documents (products, orders, etc.)
    /// </summary>
    public string ContainerName { get; set; } = default!;
}