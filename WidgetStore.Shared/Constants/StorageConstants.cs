namespace WidgetStore.Shared.Constants
{
    /// <summary>
    /// Constants related to Azure Storage operations
    /// </summary>
    public static class StorageConstants
    {
        /// <summary>
        /// Default partition key for users table
        /// </summary>
        public const string UserPartitionKey = "USER";

        /// <summary>
        /// Default partition key for reviews table
        /// </summary>
        public const string ReviewPartitionKey = "REVIEW";

        /// <summary>
        /// Local development connection string for Azurite
        /// </summary>
        public const string LocalStorageConnectionString =
            "UseDevelopmentStorage=true";
    }
}