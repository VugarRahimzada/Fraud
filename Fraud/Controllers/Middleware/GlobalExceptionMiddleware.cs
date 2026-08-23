using Fraud.Core.Common;
using Fraud.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Fraud.Controllers.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            context.Response.ContentType = "application/json";

            var (statusCode, response) = exception switch
            {
                NotFoundException notFoundEx => (
                    HttpStatusCode.NotFound,
                    ApiResponse<object?>.FailResponse(notFoundEx.Message)),

                Fraud.Core.Exceptions.ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object?>.FailResponse("Validation failed", validationEx.Errors.ToList())),

                ConflictException conflictEx => (
                    HttpStatusCode.Conflict,
                    ApiResponse<object?>.FailResponse(conflictEx.Message)),

                BusinessException businessEx => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object?>.FailResponse(businessEx.Message)),

                _ => (
                    HttpStatusCode.InternalServerError,
                    ApiResponse<object?>.FailResponse("An unexpected error occurred."))
            };

            var sourceLocation = ExceptionLocationHelper.GetSourceLocation(exception);
            var simpleMessage = ExceptionLocationHelper.Simplify(exception);
            var sqlQuery = context.Items.TryGetValue("LastSqlQuery", out var q) ? q?.ToString() : null;

            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["SourceLocation"] = sourceLocation,
                ["SqlQuery"] = sqlQuery ?? "N/A",
                ["RequestPath"] = context.Request.Path.Value,
                ["StatusCode"] = (int)statusCode
            }))
            {
                if (statusCode == HttpStatusCode.InternalServerError)
                    _logger.LogError(exception, "{SimpleMessage} | Location: {SourceLocation} | Query: {SqlQuery}",
                        simpleMessage, sourceLocation, sqlQuery ?? "N/A");
                else
                    _logger.LogWarning(exception, "{SimpleMessage} | Location: {SourceLocation}",
                        simpleMessage, sourceLocation);
            }
            // ---- Bura qədər ----

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}