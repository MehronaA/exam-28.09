using System;

namespace Domain.Filters;

public class ProductFilter
{
    public string? Keyword { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}
