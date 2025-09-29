using System;

namespace Domain.DTOs.Extra;

public class StockAdjustmentHistoryDto
{
    public DateTime AdjustmentDate { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
    
}
