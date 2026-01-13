using ChatApp.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatApp.API.Middlewares
{
    /// <summary>
    /// Global exception handler.
    /// Converts all exceptions into safe, structured API responses.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = Guid.NewGuid().ToString();

            _logger.LogError(ex, "Unhandled exception | TraceId: {TraceId}", traceId);

            context.Response.ContentType = "application/json";

            // Controlled business error
            if (ex is AppException appEx)
            {
                context.Response.StatusCode = appEx.StatusCode;

                var response = ErrorResponse.Create(
                    errors: new[] { appEx.Code },
                    message: appEx.Message,
                    statusCode: appEx.StatusCode
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            // System failure (never expose real message)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var genericResponse = ErrorResponse.Create(
                errors: new[] { "INTERNAL_SERVER_ERROR" },
                message: "Something went wrong. Please try again later.",
                statusCode: 500
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(genericResponse));
        }
    }
}
