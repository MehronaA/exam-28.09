using System;

namespace Domain.DTOs.Sale;

public class SaleUpdateDto
{
    public int ProductId { get; set; }
    public int QuantitySold { get; set; }
}
