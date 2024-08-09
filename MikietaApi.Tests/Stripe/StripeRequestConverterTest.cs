using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Stripe;
using Shouldly;

namespace MikietaApi.Tests.Stripe;

public class StripeRequestConverterTest
{
    private static OrderedProductOrderedIngredientEntity CreateOrderedProductOrderedIngredient(int quantity,
        decimal medium, decimal large)
    {
        return new OrderedProductOrderedIngredientEntity
        {
            Quantity = quantity,
            OrderedIngredient = new OrderedIngredientEntity()
            {
                PriceMedium = medium,
                PriceLarge = large
            }
        };
    }

    private static IEnumerable<TestCaseData> Convert_Cases()
    {
        yield return new TestCaseData(12m, 1, PizzaType.Medium,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 1400)
            .SetName("Convert 01");

        yield return new TestCaseData(12m, 2, PizzaType.Medium,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 1400)
            .SetName("Convert 02");

        yield return new TestCaseData(12.54m, 1, PizzaType.Medium,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 1454)
            .SetName("Convert 03");

        yield return new TestCaseData(1m, 1, PizzaType.Large,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 400)
            .SetName("Convert 04");

        yield return new TestCaseData(1m, 1, null,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 100)
            .SetName("Convert 05");

        yield return new TestCaseData(1m, 1, PizzaType.Medium,
                new[] { CreateOrderedProductOrderedIngredient(1, 2, 3) }, 300)
            .SetName("Convert 06");
    }

    [TestCaseSource(nameof(Convert_Cases))]
    public void Convert(decimal price, int quantity, PizzaType? pizzaType,
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