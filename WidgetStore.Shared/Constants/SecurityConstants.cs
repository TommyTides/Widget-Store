namespace WidgetStore.Shared.Constants
{
    /// <summary>
    /// Constants related to security and authentication
    /// </summary>
    public static class SecurityConstants
    {
        /// <summary>
        /// Minimum length required for passwords
        /// </summary>
        public const int MinimumPasswordLength = 6;

        /// <summary>
        /// Maximum length allowed for passwords
        /// </summary>
        public const int MaximumPasswordLength = 100;

        /// <summary>
        /// Pattern for valid email addresses
        /// </summary>
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    }
}