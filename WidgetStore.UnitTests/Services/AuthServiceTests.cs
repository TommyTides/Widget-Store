using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Infrastructure.Services;
using WidgetStore.UnitTests.Helpers;
using Xunit;

namespace WidgetStore.UnitTests.Services
{
    /// <summary>
    /// Tests for the AuthService class
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly AdminConfig _adminConfig;
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes test context for AuthService tests
        /// </summary>
        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            _jwtConfig = new JwtConfig
            {
                SecretKey = "YourTestSecretKeyThatIsAtLeast32CharactersLong",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            _adminConfig = new AdminConfig
            {
                AdminSecretKey = "AdminSecretKey123!"
            };

            _authService = new AuthService(_mockUserRepository.Object, _jwtConfig, _adminConfig);
        }

        #region RegisterAsync Tests

        [Fact(DisplayName = "RegisterAsync should create new user successfully")]
        public async Task RegisterAsync_ShouldCreateNewUser_WhenValidRequest()
        {
            // Arrange
            var request = TestData.GetValidRegisterRequest();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Core.Entities.User?)null);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<Core.Entities.User>()))
                .ReturnsAsync((Core.Entities.User u) => u);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Name.Should().Be(request.Name);
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<Core.Entities.User>()), Times.Once);
        }

        [Fact(DisplayName = "RegisterAsync should create admin user successfully")]
        public async Task RegisterAsync_ShouldCreateAdminUser_WhenValidRequestWithAdminKey()
        {
            // Arrange
            var request = TestData.GetValidAdminRegisterRequest();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Core.Entities.User?)null);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<Core.Entities.User>()))
                .ReturnsAsync((Core.Entities.User u) => u);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Role.Should().Be(Core.Enums.UserRole.Admin);
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<Core.Entities.User>()), Times.Once);
        }

        [Fact(DisplayName = "RegisterAsync should throw when email exists")]
        public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
        {
            // Arrange
            var request = TestData.GetValidRegisterRequest();
            var existingUser = TestData.GetTestUser();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            var act = () => _authService.RegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<SecurityException>()
                .WithMessage("User with this email already exists");
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<Core.Entities.User>()), Times.Never);
        }

        [Fact(DisplayName = "RegisterAsync should throw when admin key is invalid")]
        public async Task RegisterAsync_ShouldThrow_WhenInvalidAdminKey()
        {
            // Arrange
            var request = TestData.GetValidAdminRegisterRequest();
            request.AdminSecretKey = "WrongKey";
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Core.Entities.User?)null);

            // Act
            var act = () => _authService.RegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<SecurityException>()
                .WithMessage("Invalid admin secret key");
            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<Core.Entities.User>()), Times.Never);
        }

        #endregion

        #region LoginAsync Tests

        [Fact(DisplayName = "LoginAsync should return token when credentials are valid")]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            // Arrange
            var request = TestData.GetValidLoginRequest();
            var user = TestData.GetTestUser();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be(user.Email);
            result.Role.Should().Be(user.Role.ToString());
        }

        [Fact(DisplayName = "LoginAsync should throw when user not found")]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var request = TestData.GetValidLoginRequest();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync((Core.Entities.User?)null);

            // Act
            var act = () => _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<SecurityException>()
                .WithMessage("Invalid email or password");
        }

        [Fact(DisplayName = "LoginAsync should throw when password is invalid")]
        public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
        {
            // Arrange
            var request = TestData.GetValidLoginRequest();
            request.Password = "WrongPassword";
            var user = TestData.GetTestUser();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var act = () => _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<SecurityException>()
                .WithMessage("Invalid email or password");
        }

        #endregion

        #region GenerateJwtToken Tests

        [Fact(DisplayName = "GenerateJwtToken should create valid JWT token")]
        public void GenerateJwtToken_ShouldCreateValidToken()
        {
            // Arrange
            var user = TestData.GetTestUser();

            // Act
            var token = _authService.GenerateJwtToken(user);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(_jwtConfig.SecretKey))
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            validatedToken.Should().NotBeNull();
            principal.Should().NotBeNull();
            principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(user.Email);
            principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(user.Role.ToString());
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(user.Id);
        }

        #endregion
    }
}