using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WidgetStore.API.Controllers;
using WidgetStore.Core.DTOs.Auth;
using WidgetStore.Core.DTOs.Common;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Services;
using WidgetStore.UnitTests.Base;
using WidgetStore.UnitTests.Helpers;
using Xunit;

namespace WidgetStore.UnitTests.Controllers
{
    /// <summary>
    /// Tests for the AuthController
    /// </summary>
    public class AuthControllerTests : ControllerTestBase
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        /// <summary>
        /// Initializes test context for AuthController tests
        /// </summary>
        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
            SetupControllerContext(_controller);
        }

        #region Register Tests

        [Fact(DisplayName = "Register should return CreatedAtAction with user details when successful")]
        public async Task Register_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var request = TestData.GetValidRegisterRequest();
            var user = TestData.GetTestUser();

            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.ActionName.Should().Be(nameof(AuthController.Register));
            createdAtResult.RouteValues?["id"].Should().Be(user.Id);

            var value = createdAtResult.Value.Should().BeAssignableTo<object>().Subject;
            var dict = value.GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(value));

            dict["Id"].Should().Be(user.Id);
            dict["Email"].Should().Be(user.Email);
            dict["Name"].Should().Be(user.Name);
            dict["Role"].Should().Be(user.Role);

            _mockAuthService.Verify(x => x.RegisterAsync(request), Times.Once);
        }

        [Fact(DisplayName = "Register should return BadRequest when service throws SecurityException")]
        public async Task Register_ShouldReturnBadRequest_WhenSecurityException()
        {
            // Arrange
            var request = TestData.GetValidRegisterRequest();
            var errorMessage = "Test security error";

            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ThrowsAsync(new SecurityException(errorMessage));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errorResponse = badRequestResult.Value.Should().BeOfType<ErrorResponse>().Subject;
            errorResponse.Message.Should().Be(errorMessage);

            _mockAuthService.Verify(x => x.RegisterAsync(request), Times.Once);
        }

        #endregion

        #region Login Tests

        [Fact(DisplayName = "Login should return OK with login response when successful")]
        public async Task Login_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var request = TestData.GetValidLoginRequest();
            var response = new LoginResponse
            {
                Token = "test-token",
                Email = request.Email,
                Role = "Customer"
            };

            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var loginResponse = okResult.Value.Should().BeOfType<LoginResponse>().Subject;

            loginResponse.Token.Should().Be(response.Token);
            loginResponse.Email.Should().Be(response.Email);
            loginResponse.Role.Should().Be(response.Role);

            _mockAuthService.Verify(x => x.LoginAsync(request), Times.Once);
        }

        [Fact(DisplayName = "Login should return BadRequest when service throws SecurityException")]
        public async Task Login_ShouldReturnBadRequest_WhenSecurityException()
        {
            // Arrange
            var request = TestData.GetValidLoginRequest();
            var errorMessage = "Invalid credentials";

            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ThrowsAsync(new SecurityException(errorMessage));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var errorResponse = badRequestResult.Value.Should().BeOfType<ErrorResponse>().Subject;
            errorResponse.Message.Should().Be(errorMessage);

            _mockAuthService.Verify(x => x.LoginAsync(request), Times.Once);
        }

        #endregion

        #region Model Validation Tests

        [Fact(DisplayName = "Register should return BadRequest when model is invalid")]
        public async Task Register_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            var request = new RegisterRequest(); // Empty request
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<RegisterRequest>()), Times.Never);
        }

        [Fact(DisplayName = "Login should return BadRequest when model is invalid")]
        public async Task Login_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            var request = new LoginRequest(); // Empty request
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginRequest>()), Times.Never);
        }

        #endregion
    }
}