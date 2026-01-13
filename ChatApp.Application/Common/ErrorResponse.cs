using System.Collections.Generic;

namespace ChatApp.Application.Common
{
    /// <summary>
    /// Helper class for creating error API responses.
    /// </summary>
    public static class ErrorResponse
    {
        public static ApiResponse<object> Create(
            IEnumerable<string> errors,
            string message,
            int statusCode)
        {
            return new ApiResponse<object>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = null,
                Errors = errors
            };
        }
    }
}
