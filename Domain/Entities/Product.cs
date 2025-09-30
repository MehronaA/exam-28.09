using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    //navigation
    public Category Category { get; set; }
    public Supplier Supplier { get; set; }
    public List<Sale> Sales { get; set; }
    public List<StockAdjustment> StockAdjustments = [];

}
