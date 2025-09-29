using System;
using System.Diagnostics.Contracts;

namespace Domain.DTOs.Extra;

public class ProductsStatisticsDto
{
    public int TotalProducts { get; set; }
    public decimal AveragePrice { get; set; }
    public int TotalSold { get; set; }
    

}
