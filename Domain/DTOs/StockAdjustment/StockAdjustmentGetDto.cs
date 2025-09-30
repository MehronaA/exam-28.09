using System;
using Domain.DTOs.Products;
using Domain.Entities;

namespace Domain.DTOs.StockAdjustment;

public class StockAdjustmentGetDto
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int AdjustmentAmount { get; set; }
    public string Reason { get; set; }
    public DateTime AdjustmentDate { get; set; } 

    public Product Product { get; set; }

}       
