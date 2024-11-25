using Microsoft.AspNetCore.Mvc;
using WidgetStore.Core.DTOs.Auth;
using WidgetStore.Core.DTOs.Common;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related operations
    /// </summary>
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="authService">The authentication service</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>The created user details</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);
                return CreatedAtAction(
                    nameof(Register),
                    new { id = user.Id },
                    new { user.Id, user.Email, user.Name, user.Role });
            }
            catch (SecurityException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and user details</returns>
        /// <response code="200">Returns the JWT token and user details</response>
        /// <response code="400">If the credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (SecurityException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
        }
    }
}