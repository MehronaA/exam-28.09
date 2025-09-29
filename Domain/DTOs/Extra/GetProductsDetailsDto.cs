using System;
using System.Diagnostics.Contracts;
using Domain.DTOs.Sale;
using Domain.DTOs.StockAdjustment;
using Domain.Entities;

namespace Domain.DTOs.Extra;

public class GetProductsDetailsDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public string Supplier { get; set; }
    public IEnumerable<SaleGetDto> Sales { get; set; } = [];
    public IEnumerable<StockAdjustmentGetDto> StockAdjustments { get; set; } = [];

}
