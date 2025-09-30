using System;
using Domain.DTOs.Products;
using Domain.Entities;

namespace Domain.DTOs.Categories;

public class CategoryWithProducts
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ProductGetDto> Products = [];
}
