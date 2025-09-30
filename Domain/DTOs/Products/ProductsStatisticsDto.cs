using System;
using System.Diagnostics.Contracts;

namespace Domain.DTOs.Products;

public class ProductsStatisticsDto
{
    public int TotalProducts { get; set; }
    public decimal AveragePrice { get; set; }
    public int TotalSold { get; set; }
    

}
