using System;

namespace Domain.DTOs.StockAdjustment;

public class StockAdjustmentUpdateDto
{
    public int ProductId { get; set; }
    public int AdjustmentAmount { get; set; }
    public string Reason { get; set; }
}
