namespace WidgetStore.Core.DTOs.Auth
{
    /// <summary>
    /// Represents a login request from a user
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}