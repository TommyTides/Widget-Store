using System.Net;
using Azure;
using System.Text.Json;
using WidgetStore.Core.DTOs.Common;
using WidgetStore.Core.Exceptions;

namespace WidgetStore.API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the ErrorHandlingMiddleware
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="environment">Host environment information</param>
        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context">The HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var errorDetails = new ErrorDetails();

            switch (exception)
            {
                case SecurityException:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Message = exception.Message;
                    break;

                case BadRequestException:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Message = exception.Message;
                    break;

                case NotFoundException:
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails.Message = exception.Message;
                    break;

                case RequestFailedException ex:
                    errorDetails.StatusCode = ex.Status;
                    errorDetails.Message = "Azure Storage operation failed";
                    errorDetails.Details = _environment.IsDevelopment() ? ex.Message : null;
                    break;

                case UnauthorizedAccessException:
                    errorDetails.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorDetails.Message = "Unauthorized access";
                    break;

                default:
                    errorDetails.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetails.Message = "An internal server error occurred.";
                    errorDetails.Details = _environment.IsDevelopment() ? exception.Message : null;
                    break;
            }

            context.Response.StatusCode = errorDetails.StatusCode;
            await context.Response.WriteAsync(errorDetails.ToString());
        }
    }

    /// <summary>
    /// Extension methods for the ErrorHandlingMiddleware
    /// </summary>
    public static class ErrorHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the error handling middleware to the application pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseErrorHandling(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}