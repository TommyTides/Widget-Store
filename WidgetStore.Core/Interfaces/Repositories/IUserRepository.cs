using WidgetStore.Core.Entities;

namespace WidgetStore.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for user repository operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user in the storage
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created user entity</returns>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User if found, null otherwise</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User if found, null otherwise</returns>
        Task<User?> GetByIdAsync(string id);
    }
}