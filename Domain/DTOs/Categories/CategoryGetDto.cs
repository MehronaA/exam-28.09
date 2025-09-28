using System;
using Domain.Entities;


namespace Domain.DTOs.Categories;

public class CategoryGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
