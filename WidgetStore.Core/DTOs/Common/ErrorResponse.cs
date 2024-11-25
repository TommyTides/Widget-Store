namespace WidgetStore.Core.DTOs.Common
{
    /// <summary>
    /// Represents an error response from the API
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error details (optional)
        /// </summary>
        public string? Details { get; set; }
    }
}