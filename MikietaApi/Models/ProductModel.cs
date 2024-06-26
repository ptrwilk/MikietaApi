﻿using System.Text.Json.Serialization;

namespace MikietaApi.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IngredientModel[] Ingredients { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductType ProductType { get; set; }
        public string? ImageUrl { get; set; }
        public IReadOnlyDictionary<PizzaType, double> PizzaSizePrice { get; set; } = null!;
    }

    public class AdminOrderedProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductType ProductType { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PizzaType? PizzaType { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool Ready { get; set; }
        public AdditionalIngredientModel[] AdditionalIngredients { get; set; }
        public RemovedIngredientModel[] RemovedIngredients { get; set; }
        public ReplacedIngredientModel[] ReplacedIngredients { get; set; }
    }

    public class AdminProductModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Price { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductType ProductType { get; set; }
        public IReadOnlyDictionary<PizzaType, double> PizzaSizePrice { get; set; } = null!;
        public IngredientModel[] Ingredients { get; set; } = null!;
        public Guid? ImageId { get; set; }
        public string? ImageUrl { get; set; }
    }
}