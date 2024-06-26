﻿namespace MikietaApi.Data.Entities;

public class OrderedProductOrderedIngredientEntity
{
    public Guid OrderedProductId { get; set; }
    public OrderedProductEntity OrderedProduct { get; set; } = null!;
    public Guid OrderedIngredientId { get; set; }
    public OrderedIngredientEntity OrderedIngredient { get; set; } = null!;
    public int Quantity { get; set; }
    public bool IsAdditionalIngredient { get; set; }
    public bool IsIngredientRemoved { get; set; }
    public Guid? ReplacedIngredientId { get; set; }
    public OrderedIngredientEntity? ReplacedIngredient { get; set; }
}