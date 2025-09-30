using System;

namespace Domain.Filters;

public class SaleFilter
{
    public string? Keyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 5;
}
