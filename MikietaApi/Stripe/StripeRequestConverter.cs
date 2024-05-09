using MikietaApi.Converters;
using MikietaApi.Data.Entities;

namespace MikietaApi.Stripe;

public class StripeRequestConverter : IConverter<OrderProductEntity, StripeRequestModel>
{
    public StripeRequestModel Convert(OrderProductEntity source)
    {
        return new StripeRequestModel
        {
            Price = (int)(source.Product.Price * 100),
            Quantity = source.Quantity
        };
    }
}