using System;
using Domain.Entities;

namespace Domain.DTOs.Extra;

public class CategoryWithProducts
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Product> Products = [];
}
