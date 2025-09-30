using System;

namespace Domain.Filters;

public class CategoryFilter
{
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
