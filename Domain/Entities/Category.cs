using System;

namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    //navigations
    public IEnumerable<Product> Products = [];
    
}
