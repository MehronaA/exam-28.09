using System;
using Infrastructure.Enum;

namespace Infrastructure.APIResult;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ErrorType ErrorType { get; set; }


    public static Result<T> Ok(T? data, string? message = null)
        => new()
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };

    public static Result<T> Fail(string message, ErrorType errorType = ErrorType.Internal)
        => new()
        {
            IsSuccess = false,
            Message = message,
            ErrorType = errorType
        };
}
