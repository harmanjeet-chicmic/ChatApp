namespace ChatApp.Application.Common
{
    /// <summary>
    /// Helper class for creating successful API responses.
    /// </summary>
    public static class SuccessResponse
    {
        public static ApiResponse<T> Create<T>(
            T data,
            string message,
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = null
            };
        }
    }
}
