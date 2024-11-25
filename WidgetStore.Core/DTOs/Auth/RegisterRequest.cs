using WidgetStore.Core.Enums;

namespace WidgetStore.Core.DTOs.Auth
{
    /// <summary>
    /// Represents a user registration request
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User's address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// User's role (optional, defaults to Customer)
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Customer;

        /// <summary>
        /// Admin registration secret key (required only for admin registration)
        /// </summary>
        public string? AdminSecretKey { get; set; }
    }
}