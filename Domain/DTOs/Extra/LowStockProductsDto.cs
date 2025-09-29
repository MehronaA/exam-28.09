using System;

namespace Domain.DTOs.Extra;

public class LowStockProductsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int QuantityInStock { get; set; }

}
