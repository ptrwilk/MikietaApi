using MikietaApi.Converters;
using MikietaApi.Data.Entities;

namespace MikietaApi.Stripe;

public class StripeRequestConverter : IConverter<OrderOrderedProductEntity, StripeRequestModel>
{
    public StripeRequestModel Convert(OrderOrderedProductEntity source)
    {
        return new StripeRequestModel
        {
            Price = (int)(source.OrderedProduct.Price * 100),
            Quantity = source.Quantity,
            Name = source.OrderedProduct.Name
        };
    }
}