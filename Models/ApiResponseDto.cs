namespace Assignment6.Models.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponseDto()
        {
            Success = false;
            Data = default;
            Message = null;
            Errors = null;
        }

        public ApiResponseDto(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
            Errors = null;
        }

        public ApiResponseDto(string message, List<string>? errors = null)
        {
            Success = false;
            Data = default;
            Message = message;
            Errors = errors;
        }
    }
}
