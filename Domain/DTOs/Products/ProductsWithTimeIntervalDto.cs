using System;

namespace Domain.DTOs.Products;

public class ProductsWithTimeIntervalDto
{
    public int Id { get; set; }
    public string ProductName{ get; set; }
    public int QuantitySold { get; set; }
    public DateTime SaleDate { get; set; } 
}
