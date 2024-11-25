using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WidgetStore.Core.DTOs.Test;

namespace WidgetStore.API.Controllers
{
    /// <summary>
    /// Controller for testing authentication and authorization
    /// </summary>
    public class TestController : BaseApiController
    {
        /// <summary>
        /// Public endpoint that doesn't require authentication
        /// </summary>
        /// <returns>Test message</returns>
        [HttpGet("public")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TestResponse), StatusCodes.Status200OK)]
        public IActionResult PublicEndpoint()
        {
            return Ok(new TestResponse
            {
                Message = "This is a public endpoint - no authentication required"
            });
        }

        /// <summary>
        /// Protected endpoint that requires authentication
        /// </summary>
        /// <returns>Test message with user information</returns>
        [HttpGet("protected")]
        [Authorize]
        [ProducesResponseType(typeof(TestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ProtectedEndpoint()
        {
            var userInfo = GetCurrentUserInfo();
            return Ok(new TestResponse
            {
                Message = "This is a protected endpoint - authentication required",
                User = userInfo
            });
        }

        /// <summary>
        /// Admin endpoint that requires authentication and admin role
        /// </summary>
        /// <returns>Test message with admin user information</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult AdminEndpoint()
        {
            var userInfo = GetCurrentUserInfo();
            return Ok(new TestResponse
            {
                Message = "This is an admin endpoint - admin role required",
                User = userInfo
            });
        }

        /// <summary>
        /// Customer endpoint that requires authentication and customer role
        /// </summary>
        /// <returns>Test message with customer user information</returns>
        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(TestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult CustomerEndpoint()
        {
            var userInfo = GetCurrentUserInfo();
            return Ok(new TestResponse
            {
                Message = "This is a customer endpoint - customer role required",
                User = userInfo
            });
        }

        private UserInfo GetCurrentUserInfo()
        {
            return new UserInfo
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
            };
        }
    }
}