namespace WidgetStore.Core.Configuration
{
    /// <summary>
    /// Configuration settings for Azure Storage
    /// </summary>
    public class AzureStorageConfig
    {
        /// <summary>
        /// Connection string for Azure Storage account
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Table name for storing user data
        /// </summary>
        public string UsersTableName { get; set; } = "Users";

        /// <summary>
        /// Table name for storing reviews
        /// </summary>
        public string ReviewsTableName { get; set; } = "Reviews";

        /// <summary>
        /// Container name for storing product images
        /// </summary>
        public string ProductImagesContainer { get; set; } = "product-images";
    }
}