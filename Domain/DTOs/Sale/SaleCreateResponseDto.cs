using System;

namespace Domain.DTOs.Sale;

public class SaleCreateResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int QuantitySold { get; set; }
    public DateTime SaleDate { get; set; } 
}
