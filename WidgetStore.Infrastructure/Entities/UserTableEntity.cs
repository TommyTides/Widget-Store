using Azure;
using Azure.Data.Tables;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Enums;
using WidgetStore.Shared.Constants;

namespace WidgetStore.Infrastructure.Entities
{
    /// <summary>
    /// Represents a user entity in Azure Table Storage
    /// </summary>
    public class UserTableEntity : ITableEntity
    {
        /// <summary>
        /// Partition key for the table entity
        /// </summary>
        public string PartitionKey { get; set; } = StorageConstants.UserPartitionKey;

        /// <summary>
        /// Row key for the table entity (user's ID)
        /// </summary>
        public string RowKey { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the entity
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// ETag for concurrency handling
        /// </summary>
        public ETag ETag { get; set; }

        /// <summary>
        /// User's email address
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
        /// User's role
        /// </summary>
        public string Role { get; set; } = UserRole.Customer.ToString();

        /// <summary>
        /// User's address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if email is verified
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last modified timestamp
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Converts a domain User entity to a UserTableEntity
        /// </summary>
        /// <param name="user">The domain user entity</param>
        /// <returns>A UserTableEntity</returns>
        public static UserTableEntity FromUser(User user)
        {
            return new UserTableEntity
            {
                RowKey = user.Id,
                Email = user.Email,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                Role = user.Role.ToString(),
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };
        }

        /// <summary>
        /// Converts a UserTableEntity to a domain User entity
        /// </summary>
        /// <returns>A User domain entity</returns>
        public User ToUser()
        {
            return new User
            {
                Id = RowKey,
                Email = Email,
                Name = Name,
                PasswordHash = PasswordHash,
                Role = Enum.Parse<UserRole>(Role),
                Address = Address,
                PhoneNumber = PhoneNumber,
                IsEmailVerified = IsEmailVerified,
                CreatedAt = CreatedAt,
                ModifiedAt = ModifiedAt
            };
        }
    }
}