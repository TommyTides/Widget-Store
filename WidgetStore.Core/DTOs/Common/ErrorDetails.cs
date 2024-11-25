using System.Text.Json;

namespace WidgetStore.Core.DTOs.Common
{
    /// <summary>
    /// Represents detailed error information for API responses
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional error details (optional)
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Converts the error details to a JSON string
        /// </summary>
        /// <returns>JSON representation of the error details</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}