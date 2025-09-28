using System;

namespace Domain.DTOs.Products;

public class ProductUpdateDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }

}
