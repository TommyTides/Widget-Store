using System.Text.RegularExpressions;
using WidgetStore.Shared.Constants;

namespace WidgetStore.Shared.Helpers
{
    /// <summary>
    /// Provides validation methods for user input
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates a password against security requirements
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>True if the password meets requirements, false otherwise</returns>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < SecurityConstants.MinimumPasswordLength)
                return false;

            if (password.Length > SecurityConstants.MaximumPasswordLength)
                return false;

            return true;
        }

        /// <summary>
        /// Validates an email address format
        /// </summary>
        /// <param name="email">The email to validate</param>
        /// <returns>True if the email is valid, false otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(email, SecurityConstants.EmailPattern);
        }
    }
}