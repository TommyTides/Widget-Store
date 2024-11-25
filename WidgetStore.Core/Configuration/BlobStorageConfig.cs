namespace WidgetStore.Core.Configuration
{
    /// <summary>
    /// Configuration for Azure Blob Storage
    /// </summary>
    public class BlobStorageConfig
    {
        /// <summary>
        /// Maximum allowed file size in megabytes
        /// </summary>
        public const int MaxFileSizeInMB = 50;

        /// <summary>
        /// Allowed content types for images
        /// </summary>
        public static readonly string[] AllowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/gif"
        };
    }
}