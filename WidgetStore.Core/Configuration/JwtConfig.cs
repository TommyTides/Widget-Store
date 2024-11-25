namespace WidgetStore.Core.Configuration
{
    /// <summary>
    /// Configuration settings for JWT authentication
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// Secret key used to sign the JWT token
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Issuer of the JWT token
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audience of the JWT token
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}