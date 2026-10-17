using FSP_Images.Models;

public class ApiResult<T>
{
    public T? Data { get; set; }
    public ApiErrorResponse? ApiError { get; set; }

    public bool Success => Data is not null && ApiError is null;
}