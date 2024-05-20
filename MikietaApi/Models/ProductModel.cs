﻿namespace MikietaApi.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string[] Ingredients { get; set; }
    public double Price { get; set; }
    public ProductType ProductType { get; set; }
}

public class AdminProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ProductType Type { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public bool Ready { get; set; }
}