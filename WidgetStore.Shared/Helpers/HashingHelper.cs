using System.Security.Cryptography;
using System.Text;

namespace WidgetStore.Shared.Helpers
{
    /// <summary>
    /// Provides methods for hashing and verifying passwords
    /// </summary>
    public static class HashingHelper
    {
        /// <summary>
        /// Creates a hash from a password using SHA256
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>The hashed password</returns>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convert the password to bytes
                var passwordBytes = Encoding.UTF8.GetBytes(password);

                // Create the hash
                var hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a string
                var hash = Convert.ToBase64String(hashBytes);

                return hash;
            }
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="hash">The hash to verify against</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            // Hash the input password
            var inputHash = HashPassword(password);

            // Compare the hashes
            return inputHash.Equals(hash);
        }
    }
}