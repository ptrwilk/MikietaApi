using Stripe.Checkout;

namespace MikietaApi.Stripe;

public class StripeResponseModel
{
    public string SessionId { get; set; } = null!;
    public string Url { get; set; } = null!;
}

public class StripeRequestModel
{
    public string Name { get; set; } = null!;
    public int Price { get; set; }
    public int Quantity { get; set; }
}

public class StripeFacade
{
    private readonly string _successUrl;
    private readonly string _cancelUrl;

    public StripeFacade(string successUrl, string cancelUrl)
    {
        _successUrl = successUrl;
        _cancelUrl = cancelUrl;
    }

    public StripeResponseModel CreateSession(StripeRequestModel[] models, double? deliveryPrice)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = _successUrl,
            CancelUrl = _cancelUrl,
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            PaymentMethodTypes = new List<string>
            {
                "blik", "card", "p24", "paypal"
            },
            Locale = "pl"
        };

        foreach (var model in models)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = model.Price,
                    Currency = "pln",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = model.Name,
                    },
                },
                Quantity = model.Quantity,
            });
        }

        if (deliveryPrice.HasValue)
        {
            options.LineItems.Add(new SessionLineItemOptions()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (int)deliveryPrice.Value * 100,
                    Currency = "pln",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Dostawa",
                    }
                },
                Quantity = 1
            });
        }

        var service = new SessionService();
        var session = service.Create(options);

        return new StripeResponseModel
        {
            SessionId = session.Id,
            Url = session.Url
        };
    }
}