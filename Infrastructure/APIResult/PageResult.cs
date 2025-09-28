using System;

namespace Infrastructure.APIResult;

public class PageResult<T> :Result<T>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage { get; set; }
    
    public static PageResult<T> Ok(T? data, int page, int size, int totalCount, string? message = null)
        => new()
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            Page = page,
            Size = size,
            TotalCount = totalCount,
            TotalPage = (int)Math.Ceiling((double)totalCount / size)
        };
}
