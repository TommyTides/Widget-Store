using WidgetStore.Core.Enums;

namespace WidgetStore.Core.Entities
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// User's email address, used for authentication
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's role in the system
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Customer;

        /// <summary>
        /// User's shipping address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the user's email has been verified
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;
    }
}