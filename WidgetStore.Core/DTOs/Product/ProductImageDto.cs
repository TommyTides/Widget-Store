namespace WidgetStore.Core.DTOs.Product
{
    /// <summary>
    /// DTO for product image operations
    /// </summary>
    public class ProductImageDto
    {
        /// <summary>
        /// URL of the image in blob storage
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Content type of the image
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
    }
}