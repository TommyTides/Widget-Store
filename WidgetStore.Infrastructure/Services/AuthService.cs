using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.DTOs.Auth;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Enums;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Core.Interfaces.Services;
using WidgetStore.Shared.Helpers;

namespace WidgetStore.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for authentication operations
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly AdminConfig _adminConfig;

        /// <summary>
        /// Initializes a new instance of the AuthService class
        /// </summary>
        /// <param name="userRepository">Repository for user operations</param>
        /// <param name="jwtConfig">JWT configuration settings</param>
        /// <param name="adminConfig">Admin configuration settings</param>
        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtConfig> jwtConfig,
            IOptions<AdminConfig> adminConfig)
        {
            _userRepository = userRepository;
            _jwtConfig = jwtConfig.Value;
            _adminConfig = adminConfig.Value;
        }

        /// <inheritdoc/>
        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            // Validate email and password
            if (!ValidationHelper.IsValidEmail(request.Email))
                throw new SecurityException("Invalid email format");

            if (!ValidationHelper.IsValidPassword(request.Password))
                throw new SecurityException("Invalid password format");

            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new SecurityException("User with this email already exists");

            // Validate admin registration
            if (request.Role == UserRole.Admin)
            {
                if (string.IsNullOrEmpty(request.AdminSecretKey) ||
                    request.AdminSecretKey != _adminConfig.AdminSecretKey)
                {
                    throw new SecurityException("Invalid admin secret key");
                }
            }

            // Create new user
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = HashingHelper.HashPassword(request.Password),
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };

            // Save user
            return await _userRepository.CreateAsync(user);
        }

        /// <inheritdoc/>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new SecurityException("Invalid email or password");

            // Verify password
            if (!HashingHelper.VerifyPassword(request.Password, user.PasswordHash))
                throw new SecurityException("Invalid email or password");

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        /// <inheritdoc/>
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.MaxValue, // No expiration for POC
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}