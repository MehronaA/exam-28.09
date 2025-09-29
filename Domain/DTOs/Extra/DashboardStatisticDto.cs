using System;
using System.Runtime.InteropServices.Marshalling;

namespace Domain.DTOs.Extra;

public class DashboardStatisticDto
{
    public int TotalProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalSales { get; set; }
}
