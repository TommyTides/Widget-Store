using Azure;
using Azure.Data.Tables;
using WidgetStore.Infrastructure.Entities;
using WidgetStore.Shared.Constants;

namespace WidgetStore.UnitTests.Helpers
{
    /// <summary>
    /// Helper class for creating test table entities
    /// </summary>
    public static class TableEntityHelper
    {
        /// <summary>
        /// Creates a test UserTableEntity
        /// </summary>
        /// <param name="id">Optional ID for the entity</param>
        /// <returns>A UserTableEntity for testing</returns>
        public static UserTableEntity CreateTestUserTableEntity(string? id = null)
        {
            return new UserTableEntity
            {
                PartitionKey = StorageConstants.UserPartitionKey,
                RowKey = id ?? Guid.NewGuid().ToString(),
                Email = "test@example.com",
                Name = "Test User",
                PasswordHash = "hashedPassword",
                Role = "Customer",
                Address = "123 Test St",
                PhoneNumber = "1234567890",
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = null,
                ETag = new ETag("testEtag"),
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}