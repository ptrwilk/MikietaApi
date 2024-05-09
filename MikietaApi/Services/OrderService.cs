using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Stripe;

namespace MikietaApi.Services;

public class OrderResponseModel
{
    public string SessionId { get; set; } = null!;
    public string Url { get; set; } = null!;
}

public interface IOrderService
{
    OrderResponseModel Order(OrderModel model);
    int OrderSuccess(string sessionId);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly StripeFacade _stripe;
    private readonly IConverter<OrderProductEntity, StripeRequestModel> _converter;

    public OrderService(DataContext context, StripeFacade stripe,
        IConverter<OrderProductEntity, StripeRequestModel> converter)
    {
        _context = context;
        _stripe = stripe;
        _converter = converter;
    }

    public OrderResponseModel Order(OrderModel model)
    {
        var products = _context.Products
            .Where(x => model.ProductQuantities.Select(g => g.ProductId).Any(z => x.Id == z)).ToArray();

        //TODO: napisać validację sprawdzająca czy każdy productId znajduje sie w Products

        var entity = new OrderEntity
        {
            DeliveryMethod = model.DeliveryMethod,
            DeliveryRightAway = model.DeliveryRightAway,
            Name = model.Name,
            Comments = model.Comments,
            Email = model.Email,
            Nip = model.Nip,
            Phone = model.Phone,
            City = model.City,
            Street = model.Street,
            FlatNumber = model.FlatNumber,
            Floor = model.Floor,
            HomeNumber = model.HomeNumber,
            DeliveryTiming = model.DeliveryTiming,
            PaymentMethod = model.PaymentMethod,
            ProcessingPersonalDataByEmail = model.ProcessingPersonalData?.Email,
            ProcessingPersonalDataBySms = model.ProcessingPersonalData?.Sms
        };

        entity.OrderProducts = products.Select(x => new OrderProductEntity
        {
            Order = entity,
            Product = x,
            Quantity = model.ProductQuantities.First(z => z.ProductId == x.Id).Quantity
        }).ToList();

        _context.Orders.Add(entity);
        
        var res = _stripe.CreateSession(entity.OrderProducts.Select(_converter.Convert).ToArray());
        
        entity.SessionId = res.SessionId;

        _context.SaveChanges();

        return new OrderResponseModel
        {
            SessionId = res.SessionId,
            Url = res.Url
        };
    }

    public int OrderSuccess(string sessionId)
    {
        var entity = _context.Orders.FirstOrDefault(x => x.SessionId == sessionId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Order with sessionId {sessionId} not found.");
        }
        if (entity.Paid)
        {
            throw new Exception("SessionId is no longer valid.");
        }

        entity.Paid = true;

        _context.SaveChanges();

        return entity.Number;
    }
}