using System;

namespace Domain.DTOs.Sale;

public class SaleCreateDto
{
    public int ProductId { get; set; }
    public int QuantitySold { get; set; }
}
