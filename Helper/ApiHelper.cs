namespace Job.Helpers
{
    public static class ResponseHelper
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail<T>(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message
            };
        }

        public static ApiResponse<T> Fail<T>(List<string> errors, string message = "Validation Failed")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string>? Errors { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}