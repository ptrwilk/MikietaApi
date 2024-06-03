using MikietaApi.Data.Entities;
using MikietaApi.Stripe;

namespace MikietaApi.Tests.Stripe;

public class StripeRequestConverterTest
{
    [TestCase(12d, ExpectedResult = 1200)]
    [TestCase(12.54d, ExpectedResult = 1254)]
    [TestCase(1d, ExpectedResult = 100)]
    public int Convert_ShouldReturnProperPrice(double price)
    {
        var converter = new StripeRequestConverter();

        var res = converter.Convert(new OrderOrderedProductEntity
        {
            OrderedProduct = new OrderedProductEntity
            {
                Price = price,
            }
        });

        return res.Price;
    }
}