using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Net;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Infrastructure.Entities;
using WidgetStore.Shared.Constants;

namespace WidgetStore.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for user operations using Azure Table Storage
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly TableClient _tableClient;

        /// <summary>
        /// Initializes a new instance of the UserRepository class
        /// </summary>
        /// <param name="tableClient">Table client for accessing Azure Table Storage</param>
        public UserRepository(IOptions<AzureStorageConfig> storageConfig)
        {
            var tableServiceClient = new TableServiceClient(storageConfig.Value.ConnectionString);
            _tableClient = tableServiceClient.GetTableClient(storageConfig.Value.UsersTableName);
            _tableClient.CreateIfNotExists();
        }

        /// <summary>
        /// Creates a new user in the storage
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        public async Task<User> CreateAsync(User user)
        {
            var tableEntity = UserTableEntity.FromUser(user);
            await _tableClient.AddEntityAsync(tableEntity);
            return user;
        }

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User if found, null otherwise</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var queryResults = _tableClient.QueryAsync<UserTableEntity>(
                    filter: $"Email eq '{email}'"
                );

                await foreach (var entity in queryResults)
                {
                    return entity.ToUser();
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                return null;
            }
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User if found, null otherwise</returns>
        public async Task<User?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<UserTableEntity>(
                    StorageConstants.UserPartitionKey,
                    id
                );

                return response.Value.ToUser();
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}