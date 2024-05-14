using Microsoft.EntityFrameworkCore;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Stripe;

namespace MikietaApi.Services;

public class OrderResponseModel2
{
    public string SessionId { get; set; } = null!;
    public string Url { get; set; } = null!;
}

public interface IOrderService
{
    OrderResponseModel2 Order(OrderModel model);
    int OrderSuccess(string sessionId);
    OrderResponseModel[] GetAll();
    SingleProductModel[] Get(int orderId);
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

    public OrderResponseModel2 Order(OrderModel model)
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
            DeliveryTiming = model.DeliveryTiming.Value.ToLocalTime(),
            PaymentMethod = model.PaymentMethod,
            ProcessingPersonalDataByEmail = model.ProcessingPersonalData?.Email,
            ProcessingPersonalDataBySms = model.ProcessingPersonalData?.Sms,
            CreatedAt = DateTime.Now
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

        return new OrderResponseModel2
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

    public OrderResponseModel[] GetAll()
    {
        return _context.Orders.Include(x => x.OrderProducts)
            .ThenInclude(x => x.Product).Select(x => new OrderResponseModel
        {
            Id = x.Id,
            Name = x.Name,
            Address = $"{x.City}",
            Cost = x.OrderProducts.Where(z => z.OrderId == x.Id).Sum(z => z.Product.Price * z.Quantity),
            Phone = x.Phone,
            Number = x.Number,
            Payed = x.Paid,
            Status = OrderStatusType.Preparing,
            OnSitePickup = x.DeliveryMethod == DeliveryMethodType.TakeAway,
            TotalProducts = x.OrderProducts.Count(z => z.OrderId == x.Id),
            CreatedAt = x.CreatedAt ?? new DateTime(),
            DeliveryAt = x.DeliveryTiming ?? new DateTime()
        }).ToArray();
    }

    public SingleProductModel[] Get(int orderId)
    {
        return _context.OrderProducts.Include(x => x.Product)
            .Where(x => x.OrderId == orderId)
            .Select(g => new SingleProductModel
            {
                Id = g.Product.Id,
                Name = g.Product.Name,
                Price = g.Product.Price * g.Quantity,
                Type = EnumConverter.Convert(g.Product.ProductType),
                Quantity = g.Quantity,
            })
            .ToArray();
        
        return new[]
        {
            new SingleProductModel
            {
                Id = 1,
                Name = "Pizza Margaritha",
                Quantity = 2,
                Ready = false,
                Price = 33.3,
                Type = ProductType.PizzaBig
            },
            new SingleProductModel
            {
                Id = 2,
                Name = "Pizza Capricosa",
                Quantity = 1,
                Ready = true,
                Price = 12.3,
                Type = ProductType.PizzaSmall
            }
        };
    }
}