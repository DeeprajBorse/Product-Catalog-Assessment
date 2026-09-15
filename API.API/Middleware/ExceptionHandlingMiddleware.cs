using System.Net;
using System.Text.Json;
using API.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = API.Domain.Exceptions.ValidationException;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var problemDetails = MapExceptionToProblemDetails(exception, context);

            if (problemDetails.Status >= 500)
            {
                _logger.LogError(exception, "Unhandled server error. TraceId: {TraceId}", traceId);
            }
            else
            {
                _logger.LogWarning(exception, "Handled client error ({StatusCode}). TraceId: {TraceId}", problemDetails.Status, traceId);
            }

            // RFC 7807 requires standard MIME type application/problem+json
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var json = JsonSerializer.Serialize(problemDetails, problemDetails.GetType(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(json);
        }

        private ProblemDetails MapExceptionToProblemDetails(Exception exception, HttpContext context)
        {
            var traceId = context.TraceIdentifier;
            var instancePath = context.Request.Path;

            return exception switch
            {
                ValidationException ex => new HttpValidationProblemDetails(ex.Errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = "See the errors field for details.",
                    Instance = instancePath,
                    Extensions = { ["traceId"] = traceId }
                },

                NotFoundException ex => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Resource Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = ex.Message,
                    Instance = instancePath,
                    Extensions = { ["traceId"] = traceId }
                },

                ForbiddenAccessException ex => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Forbidden",
                    Status = (int)HttpStatusCode.Forbidden,
                    Detail = ex.Message,
                    Instance = instancePath,
                    Extensions = { ["traceId"] = traceId }
                },

                UnauthorizedAccessException ex => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = ex.Message,
                    Instance = instancePath,
                    Extensions = { ["traceId"] = traceId }
                },

                _ => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = _env.IsDevelopment()
                        ? $"{exception.Message} | StackTrace: {exception.StackTrace}"
                        : "An unexpected error occurred. Please try again later.",
                    Instance = instancePath,
                    Extensions = { ["traceId"] = traceId }
                }
            };
        }
    }
}