using System;

namespace Domain.DTOs.StockAdjustment;

public class StockAdjustmentGetDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int AdjustmentAmount { get; set; }
    public string Reason { get; set; }
}       
