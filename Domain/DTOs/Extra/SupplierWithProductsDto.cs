using System;

namespace Domain.DTOs.Extra;

public class SupplierWithProductsDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public List<string> ProductName{ get; set; }
}
