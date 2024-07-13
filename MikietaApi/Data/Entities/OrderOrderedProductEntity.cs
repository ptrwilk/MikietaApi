namespace MikietaApi.Data.Entities;

public class OrderOrderedProductEntity
{
    public Guid OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public Guid OrderedProductId { get; set; }
    public OrderedProductEntity OrderedProduct { get; set; } = null!;
    public int Quantity { get; set; }
    public bool Ready { get; set; }

    public decimal CalculatePrice()
    {
        var product = OrderedProduct;
        var sum = product.OrderedProductOrderedIngredients.Sum(x =>
            product.PizzaType is null || x.IsIngredientRemoved
                ? 0
                : x.ReplacedIngredient is not null
                    ? x.ReplacedIngredient.Prices[(int)product.PizzaType] * x.Quantity
                    : x.OrderedIngredient.Prices[(int)product.PizzaType] * x.Quantity);

        return (product.Price + sum) * Quantity;
    }
}