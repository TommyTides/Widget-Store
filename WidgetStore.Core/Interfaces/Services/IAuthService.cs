using WidgetStore.Core.DTOs.Auth;
using WidgetStore.Core.Entities;

namespace WidgetStore.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for authentication operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">Registration request details</param>
        /// <returns>The registered user</returns>
        Task<User> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">Login request details</param>
        /// <returns>Login response with JWT token</returns>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Generates a JWT token for a user
        /// </summary>
        /// <param name="user">User to generate token for</param>
        /// <returns>JWT token string</returns>
        string GenerateJwtToken(User user);
    }
}
