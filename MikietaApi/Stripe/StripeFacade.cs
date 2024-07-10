using Stripe;
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

    public virtual StripeResponseModel CreateSession(StripeRequestModel[] models, double? deliveryPrice)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = _successUrl,
            CancelUrl = _cancelUrl,
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            PaymentMethodTypes = new List<string>
            {
                "blik", "card", "p24"
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

    public async Task<decimal> GetTransactionFee(string sessionId)
    {
        var service = new SessionService();
        var session = await service.GetAsync(sessionId);
        
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

        var fee = await GetTransactionFee(paymentIntent.LatestChargeId, 5);

        return fee;
    }
    
    private async Task<decimal> GetTransactionFee(string chargeId, int maxRetries)
    {
        var chargeService = new ChargeService();
        var balanceTransactionService = new BalanceTransactionService();

        for (int i = 0; i < maxRetries; i++)
        {
            var charge = await chargeService.GetAsync(chargeId);

            if (charge.Status != "succeeded")
            {
                throw new Exception($"Charge is not in succeeded state. Current status: {charge.Status}");
            }

            if (charge.BalanceTransactionId != null)
            {
                var balanceTransaction = await balanceTransactionService.GetAsync(charge.BalanceTransactionId);
                return balanceTransaction.Fee / 100.0m;
            }

            // Wait before retrying (exponential backoff)
            await Task.Delay((int)Math.Pow(2, i) * 1000);
        }

        throw new Exception("Unable to retrieve balance transaction after multiple attempts");
    }
}