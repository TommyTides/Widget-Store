using WidgetStore.Core.DTOs.Auth;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Enums;

namespace WidgetStore.UnitTests.Helpers
{
    /// <summary>
    /// Provides test data for unit tests
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// Creates a valid register request for testing
        /// </summary>
        public static RegisterRequest GetValidRegisterRequest()
        {
            return new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Test123!",
                Name = "Test User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
        }

        /// <summary>
        /// Creates a valid admin register request for testing
        /// </summary>
        public static RegisterRequest GetValidAdminRegisterRequest()
        {
            return new RegisterRequest
            {
                Email = "admin@example.com",
                Password = "Admin123!",
                Name = "Admin User",
                Address = "123 Admin St",
                PhoneNumber = "1234567890",
                Role = UserRole.Admin,
                AdminSecretKey = "AdminSecretKey123!"
            };
        }

        /// <summary>
        /// Creates a valid login request for testing
        /// </summary>
        public static LoginRequest GetValidLoginRequest()
        {
            return new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test123!"
            };
        }

        /// <summary>
        /// Creates a test user for testing
        /// </summary>
        public static User GetTestUser()
        {
            return new User
            {
                Id = "testId",
                Email = "test@example.com",
                Name = "Test User",
                PasswordHash = "hashedPassword",
                Role = UserRole.Customer,
                Address = "123 Test St",
                PhoneNumber = "1234567890",
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}