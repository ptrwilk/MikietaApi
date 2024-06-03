using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Hubs;
using MikietaApi.Models;
using MikietaApi.Stripe;

namespace MikietaApi.Services;

public class OrderResponseModel2
{
    public string? SessionId { get; set; }
    public string? Url { get; set; }
    public Guid? OrderId { get; set; }
}

public interface IOrderService
{
    OrderResponseModel2 Order(OrderModel model);
    Guid OrderSuccess(string sessionId);
    void OrderCanceled();
    AdminOrderModel[] GetAll();
    AdminProductModel[] Get(Guid orderId);
    AdminOrderModel GetSingle(Guid orderId);
    OrderStatusModel GetStatus(Guid orderId);
    AdminOrderModel Update(AdminOrderModel model);
    AdminProductModel UpdateProduct(Guid orderId, AdminProductModel model);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly StripeFacade _stripe;
    private readonly IConverter<OrderOrderedProductEntity, StripeRequestModel> _converter;
    private readonly IHubContext<MessageHub, IMessageHub> _hub;

    public OrderService(DataContext context, StripeFacade stripe,
        IConverter<OrderOrderedProductEntity, StripeRequestModel> converter,
        IHubContext<MessageHub, IMessageHub> hub)
    {
        _context = context;
        _stripe = stripe;
        _converter = converter;
        _hub = hub;
    }

    public OrderResponseModel2 Order(OrderModel model)
    {
        var products = _context.Products.Include(x => x.Ingredients)
            .Where(x => model.ProductQuantities.Select(g => g.ProductId).Any(z => x.Id == z)).ToArray();

        var orderedProducts = products.Select(ToOrderedProduct).ToArray();

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
            DeliveryTiming = model.DeliveryTiming?.ToLocalTime() ?? DateTime.Now,
            PaymentMethod = model.PaymentMethod,
            ProcessingPersonalDataByEmail = model.ProcessingPersonalData?.Email,
            ProcessingPersonalDataBySms = model.ProcessingPersonalData?.Sms,
            CreatedAt = DateTime.Now,
            Visible = model.PaymentMethod == PaymentMethodType.Cash
        };

        entity.OrderOrderedProducts = orderedProducts.Select(orderedProduct => new OrderOrderedProductEntity
        {
            Order = entity,
            OrderedProduct = orderedProduct,
            Quantity = model.ProductQuantities.First(z => z.ProductId == orderedProduct.ProductId).Quantity
        }).ToList();

        _context.Orders.Add(entity);

        var res = _stripe.CreateSession(entity.OrderOrderedProducts.Select(_converter.Convert).ToArray());

        entity.SessionId = res.SessionId;

        _context.SaveChanges();

        if (model.PaymentMethod == PaymentMethodType.Cash)
        {
            _hub.Clients.All.OrderMade();

            return new OrderResponseModel2
            {
                OrderId = entity.Id
            };
        }

        return new OrderResponseModel2
        {
            SessionId = res.SessionId,
            Url = res.Url
        };
    }

    public Guid OrderSuccess(string sessionId)
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
        entity.Visible = true;

        _context.SaveChanges();

        _hub.Clients.All.OrderMade();

        return entity.Id;
    }

    public void OrderCanceled()
    {
        _hub.Clients.All.OrderMade();
    }

    public AdminOrderModel[] GetAll()
    {
        return _context.Orders.Include(x => x.OrderOrderedProducts)
            .ThenInclude(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedIngredients)
            .Where(x => x.Visible)
            .ToList()
            .Select(Convert)
            .OrderByDescending(x => x.Number)
            .ToArray();
    }

    public AdminProductModel[] Get(Guid orderId)
    {
        return _context.OrderOrderedProducts.Include(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedIngredients)
            .Where(x => x.OrderId == orderId)
            .ToList()
            .Select(Convert)
            .ToArray();
    }

    public AdminOrderModel GetSingle(Guid orderId)
    {
        var entity = _context.Orders.Include(x => x.OrderOrderedProducts)
            .ThenInclude(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedIngredients)
            .First(x => x.Id == orderId);
        
        return Convert(entity);
    }

    public OrderStatusModel GetStatus(Guid orderId)
    {
        var entity = _context.Orders.First(x => x.Id == orderId);

        return new OrderStatusModel
        {
            Status = entity.Status,
            DeliveryAt = entity.DeliveryTiming,
            DeliveryMethod = entity.DeliveryMethod
        };
    }

    public AdminOrderModel Update(AdminOrderModel model)
    {
        var entity = _context.Orders.First(x => x.Id == model.Id);

        var deliveryAt = model.DeliveryAt.ToLocalTime();

        var canUpdate = entity.Status != model.Status || entity.DeliveryTiming != deliveryAt;

        entity.Status = model.Status;
        entity.Paid = model.Payed;
        entity.DeliveryTiming = deliveryAt;
        entity.City = model.Address.City;
        entity.Street = model.Address.Street;
        entity.Floor = model.Address.Floor;
        entity.FlatNumber = model.Address.FlatNumber;
        entity.HomeNumber = model.Address.HomeNumber;

        _context.SaveChanges();

        if (canUpdate)
        {
            _hub.Clients(model.Id).OrderChanged();
        }

        return model;
    }

    public AdminProductModel UpdateProduct(Guid orderId, AdminProductModel model)
    {
        var entity = _context.OrderOrderedProducts
            .Single(x => x.OrderId == orderId && x.OrderedProductId == model.Id);

        entity.Ready = model.Ready;

        _context.SaveChanges();

        return model;
    }

    private AdminProductModel Convert(OrderOrderedProductEntity entity)
    {
        return new AdminProductModel
        {
            Id = entity.OrderedProduct.Id,
            Name = entity.OrderedProduct.Name,
            Price = ToPrice(entity.OrderedProduct) * entity.Quantity,
            ProductType = entity.OrderedProduct.ProductType,
            PizzaType = entity.OrderedProduct.PizzaType,
            Quantity = entity.Quantity,
            Ready = entity.Ready
        };
    }

    private AdminOrderModel Convert(OrderEntity entity)
    {
        return new AdminOrderModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Address = new AdminAddressModel
            {
                City = entity.City,
                Street = entity.Street,
                FlatNumber = entity.FlatNumber,
                Floor = entity.Floor,
                HomeNumber = entity.HomeNumber
            },
             Cost = entity.OrderOrderedProducts.Where(z => z.OrderId == entity.Id).Sum(z => ToPrice(z.OrderedProduct) * z.Quantity),
            Phone = entity.Phone,
            Number = entity.Number,
            Payed = entity.Paid,
            Status = entity.Status,
            DeliveryMethod = entity.DeliveryMethod,
            TotalProducts = entity.OrderOrderedProducts.Count,
            CompletedProducts = entity.OrderOrderedProducts.Count(z => z.Ready),
            CreatedAt = entity.CreatedAt,
            DeliveryAt = entity.DeliveryTiming
        };
    }

    private double ToPrice(OrderedProductEntity entity)
    {
        var sum = entity.OrderedIngredients.Sum(x => entity.PizzaType is null ? 0 : x.Prices[(int)entity.PizzaType]);

        return entity.Price + sum;
    }

    private OrderedProductEntity ToOrderedProduct(ProductEntity entity)
    {
        return new OrderedProductEntity
        {
            ProductId = entity.Id,
            PizzaType = entity.PizzaType,
            Name = entity.Name,
            Price = entity.Price,
            Description = entity.Description,
            ProductType = entity.ProductType,
            OrderedIngredients = entity.Ingredients.Select(x => new OrderedIngredientEntity
            {
                Name = x.Name,
                PriceLarge = x.PriceLarge,
                PriceMedium = x.PriceMedium,
                PriceSmall = x.PriceSmall,
                IngredientId = x.Id,
            }).ToArray()
        };
    }
}