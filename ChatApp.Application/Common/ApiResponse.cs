namespace ChatApp.Application.Common
{
    /// <summary>
    /// Represents a standardized API response structure.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
