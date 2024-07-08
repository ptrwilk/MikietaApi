using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Stripe;
using Shouldly;

namespace MikietaApi.Tests.Stripe;

public class StripeRequestConverterTest
{
    private static OrderedProductOrderedIngredientEntity CreateOrderedProductOrderedIngredient(int quantity,
        double small, double medium, double large)
    {
        return new OrderedProductOrderedIngredientEntity
        {
            Quantity = quantity,
            OrderedIngredient = new OrderedIngredientEntity()
            {
                PriceSmall = small,
                PriceMedium = medium,
                PriceLarge = large
            }
        };
    }

    private static IEnumerable<TestCaseData> Convert_Cases()
    {
        yield return new TestCaseData(12d, 1, PizzaType.Small,
                new[] { CreateOrderedProductOrderedIngredient(1, 1, 2, 3) }, 1300)
            .SetName("Convert 01");

        yield return new TestCaseData(12d, 2, PizzaType.Small,
                new[] { CreateOrderedProductOrderedIngredient(1, 1, 2, 3) }, 1300)
            .SetName("Convert 02");

        yield return new TestCaseData(12.54d, 1, PizzaType.Medium,
                new[] { CreateOrderedProductOrderedIngredient(1, 1, 2, 3) }, 1454)
            .SetName("Convert 03");

        yield return new TestCaseData(1d, 1, PizzaType.Large,
                new[] { CreateOrderedProductOrderedIngredient(1, 1, 2, 3) }, 400)
            .SetName("Convert 04");

        yield return new TestCaseData(1d, 1, null,
                new[] { CreateOrderedProductOrderedIngredient(1, 1, 2, 3) }, 100)
            .SetName("Convert 05");
        
        yield return new TestCaseData(1d, 1, PizzaType.Small,
                new[] { CreateOrderedProductOrderedIngredient(2, 1, 2, 3) }, 300)
            .SetName("Convert 06");
    }

    [TestCaseSource(nameof(Convert_Cases))]
    public void Convert(double price, int quantity, PizzaType? pizzaType,
        OrderedProductOrderedIngredientEntity[] orderedProductOrderedIngredients,
        int expectedPrice)
    {
        var converter = new StripeRequestConverter();

        var res = converter.Convert(new OrderOrderedProductEntity
        {
            Quantity = quantity,
            OrderedProduct = new OrderedProductEntity
            {
                Price = price,
                PizzaType = pizzaType,
                OrderedProductOrderedIngredients = orderedProductOrderedIngredients
            }
        });

        res.Price.ShouldBe(expectedPrice);
    }
}