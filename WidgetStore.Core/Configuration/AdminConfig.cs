namespace WidgetStore.Core.Configuration
{
    /// <summary>
    /// Configuration settings for admin operations
    /// </summary>
    public class AdminConfig
    {
        /// <summary>
        /// Secret key required for admin registration
        /// </summary>
        public string AdminSecretKey { get; set; } = string.Empty;
    }
}