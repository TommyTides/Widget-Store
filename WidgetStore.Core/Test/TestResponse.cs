namespace WidgetStore.Core.DTOs.Test
{
    /// <summary>
    /// Response DTO for test endpoints
    /// </summary>
    public class TestResponse
    {
        /// <summary>
        /// Message returned from the test endpoint
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// User information if authenticated
        /// </summary>
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// User information for test responses
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// User's ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's role
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}