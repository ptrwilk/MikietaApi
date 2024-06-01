namespace MikietaApi.Models
{
    public class IngredientModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public IngredientType IngredientType { get; set; }
        public Dictionary<PizzaSize, decimal>? PricePerSize { get; set; }
    }

    public enum IngredientType
    {
        Cheese,
        Meat, 
        Veggie,
        FishOrSeaFood
    }
}
