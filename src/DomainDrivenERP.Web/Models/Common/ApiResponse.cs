namespace DomainDrivenERP.Web.Models.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Success(T? data)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(string errorMessage)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Errors = new List<string> { errorMessage }
        };
    }

    public static ApiResponse<T> Failure(List<string> errors)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = string.Join(", ", errors),
            Errors = errors
        };
    }
}
