namespace WidgetStore.API.Helpers
{
    /// <summary>
    /// Helper methods for testing authentication
    /// </summary>
    public static class AuthTestHelper
    {
        /// <summary>
        /// Provides test credentials for admin user
        /// </summary>
        public static class AdminUser
        {
            public const string Email = "admin@widgetstore.com";
            public const string Password = "Admin123!";
            public const string Name = "Admin";
        }

        /// <summary>
        /// Provides test credentials for customer user
        /// </summary>
        public static class CustomerUser
        {
            public const string Email = "customer@widgetstore.com";
            public const string Password = "Customer123!";
            public const string Name = "Customer";
        }
    }
}