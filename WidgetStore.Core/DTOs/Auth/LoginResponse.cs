namespace WidgetStore.Core.DTOs.Auth
{
    /// <summary>
    /// Represents the response after successful login
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT token for authentication
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's role
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}