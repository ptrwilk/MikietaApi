using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Hubs;
using MikietaApi.Models;
using MikietaApi.SendEmail;
using MikietaApi.SendEmail.Order;
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
    bool ClearCanClearBasket(Guid orderId);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly StripeFacade _stripe;
    private readonly IConverter<OrderOrderedProductEntity, StripeRequestModel> _converter;
    private readonly IHubContext<MessageHub, IMessageHub> _hub;
    private readonly IDeliveryService _deliveryService;
    private readonly IEmailSender<OrderEmailSenderModel> _emailSender;
    private readonly ConfigurationOptions _options;

    public OrderService(DataContext context, StripeFacade stripe,
        IConverter<OrderOrderedProductEntity, StripeRequestModel> converter,
        IHubContext<MessageHub, IMessageHub> hub,
        IDeliveryService deliveryService,
        IEmailSender<OrderEmailSenderModel> emailSender,
        ConfigurationOptions options)
    {
        _context = context;
        _stripe = stripe;
        _converter = converter;
        _hub = hub;
        _deliveryService = deliveryService;
        _emailSender = emailSender;
        _options = options;
    }

    //TODO: Tests checking if prices are correct after making an order
    public OrderResponseModel2 Order(OrderModel model)
    {
        var orderedProducts = CreateOrderedProducts(model);

        var deliveryPrice = _deliveryService.CheckDistance(new DeliveryModel
        {
            HomeNumber = model.HomeNumber ?? "",
            City = model.City ?? "",
            Street = model.Street ?? ""
        }).DeliveryPrice;

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
            Visible = model.PaymentMethod == PaymentMethodType.Cash,
            DeliveryPrice = deliveryPrice,
            CanClearBasket = model.PaymentMethod == PaymentMethodType.Cash
        };

        entity.OrderOrderedProducts = orderedProducts.Select(orderedProduct => new OrderOrderedProductEntity
        {
            Order = entity,
            OrderedProduct = orderedProduct,
            Quantity = model.ProductQuantities.First(z => z.ProductId == orderedProduct.ProductId).Quantity
        }).ToList();

        _context.Orders.Add(entity);

        var res = _stripe.CreateSession(entity.OrderOrderedProducts.Select(_converter.Convert).ToArray(),
            entity.DeliveryPrice);

        entity.SessionId = res.SessionId;

        _context.SaveChanges();

        if (model.PaymentMethod == PaymentMethodType.Cash)
        {
            _emailSender.Send(ConvertToEmailSender(entity));

            _hub.Clients.All.OrderMade();

            return new OrderResponseModel2
            {
                OrderId = entity.Id,
            };
        }

        return new OrderResponseModel2
        {
            SessionId = res.SessionId,
            Url = res.Url
        };
    }

    private OrderEmailSenderModel ConvertToEmailSender(OrderEntity entity)
    {
        return new OrderEmailSenderModel
        {
            Delivery = entity.DeliveryMethod == DeliveryMethodType.Delivery,
            TransferPaid = false,
            Link = $"{_options.WebsiteUrl}/zamowienie/{entity.Id}",
            DeliveryCost = entity.DeliveryPrice ?? 0,
            OrderDate = DateTime.Now,
            RecipientEmail = entity.Email,
            Products = entity.OrderOrderedProducts.Select(x => new OrderProductFragmentModel
            {
                Price = x.OrderedProduct.Price,
                Quantity = x.Quantity,
                Name = x.OrderedProduct.Name,
                Ingredients = x.OrderedProduct.OrderedProductOrderedIngredients.Select(y => y.OrderedIngredient.Name)
                    .ToArray()
            }).ToArray()
        };
    }

    public Guid OrderSuccess(string sessionId)
    {
        var entity = _context.Orders
            .Include(x => x.OrderOrderedProducts).ThenInclude(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedProductOrderedIngredients)
            .ThenInclude(x => x.OrderedIngredient)
            .FirstOrDefault(x => x.SessionId == sessionId);

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
        entity.CanClearBasket = true;

        _context.SaveChanges();

        _emailSender.Send(ConvertToEmailSender(entity));

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
            .ThenInclude(x => x.OrderedProductOrderedIngredients)
            .ThenInclude(x => x.OrderedIngredient)
            .Where(x => x.Visible)
            .ToList()
            .Select(Convert)
            .OrderByDescending(x => x.Number)
            .ToArray();
    }

    public AdminProductModel[] Get(Guid orderId)
    {
        return _context.OrderOrderedProducts.Include(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedProductOrderedIngredients)
            .ThenInclude(x => x.OrderedIngredient)
            .Include(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedProductOrderedIngredients)
            .ThenInclude(x => x.ReplacedIngredient)
            .Where(x => x.OrderId == orderId)
            .ToList()
            .Select(Convert)
            .ToArray();
    }

    public AdminOrderModel GetSingle(Guid orderId)
    {
        var entity = _context.Orders.Include(x => x.OrderOrderedProducts)
            .ThenInclude(x => x.OrderedProduct)
            .ThenInclude(x => x.OrderedProductOrderedIngredients)
            .ThenInclude(x => x.OrderedIngredient)
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
            DeliveryMethod = entity.DeliveryMethod,
            CanClearBasket = entity.CanClearBasket
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

    public bool ClearCanClearBasket(Guid orderId)
    {
        var order = _context.Orders.Single(x => x.Id == orderId);

        var flagCleared = order.CanClearBasket;
        order.CanClearBasket = false;

        if (flagCleared)
        {
            _context.SaveChanges();
        }

        return flagCleared;
    }

    private AdminProductModel Convert(OrderOrderedProductEntity entity)
    {
        var items = entity.OrderedProduct.OrderedProductOrderedIngredients.ToArray();

        return new AdminProductModel
        {
            Id = entity.OrderedProduct.Id,
            Name = entity.OrderedProduct.Name,
            Price = ToPrice(entity.OrderedProduct) * entity.Quantity,
            ProductType = entity.OrderedProduct.ProductType,
            PizzaType = entity.OrderedProduct.PizzaType,
            Quantity = entity.Quantity,
            Ready = entity.Ready,
            AdditionalIngredients = items
                .Where(x => x.IsAdditionalIngredient).Select(x => new AdditionalIngredientModel
                {
                    IngredientId = x.OrderedIngredient.IngredientId,
                    Name = x.OrderedIngredient.Name,
                    Quantity = x.Quantity
                }).ToArray(),
            RemovedIngredients = items.Where(x => x.IsIngredientRemoved).Select(x => new RemovedIngredientModel
            {
                IngredientId = x.OrderedIngredient.IngredientId,
                Name = x.OrderedIngredient.Name
            }).ToArray(),
            ReplacedIngredients = items.Where(x => x.ReplacedIngredient is not null).Select(x =>
                new ReplacedIngredientModel
                {
                    FromIngredientId = x.OrderedIngredient.IngredientId,
                    FromName = x.OrderedIngredient.Name,
                    ToIngredientId = x.ReplacedIngredient!.IngredientId,
                    ToName = x.ReplacedIngredient!.Name
                }).ToArray()
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
            Cost = GetOrderPrice(entity),
            Phone = entity.Phone,
            Number = entity.Number,
            Payed = entity.Paid,
            Status = entity.Status,
            DeliveryMethod = entity.DeliveryMethod,
            TotalProducts = entity.OrderOrderedProducts.Count,
            CompletedProducts = entity.OrderOrderedProducts.Count(z => z.Ready),
            CreatedAt = entity.CreatedAt,
            DeliveryAt = entity.DeliveryTiming,
            DeliveryPrice = entity.DeliveryPrice
        };
    }

    private double GetOrderPrice(OrderEntity entity)
    {
        return entity.OrderOrderedProducts.Where(z => z.OrderId == entity.Id)
            .Sum(z => ToPrice(z.OrderedProduct) * z.Quantity) + (entity.DeliveryPrice ?? 0);
    }

    private OrderedProductEntity[] CreateOrderedProducts(OrderModel model)
    {
        //TODO: napisać validację sprawdzająca czy każdy productId znajduje sie w Products
        var products = _context.Products.Include(x => x.Ingredients)
            .Where(x => model.ProductQuantities.Select(g => g.ProductId).Any(z => x.Id == z)).ToArray();

        //TODO: napisać validację sprawdzająca czy każdy ingredientId znajduje sie w Ingredients
        var additionalIngredients = model.ProductQuantities
            .SelectMany(z => z.AdditionalIngredients ?? Array.Empty<AdditionalIngredientModel>())
            .Select(g => g.IngredientId).ToArray();
        var removedIngredients = model.ProductQuantities
            .SelectMany(z => z.RemovedIngredients ?? Array.Empty<RemovedIngredientModel>())
            .Select(x => x.IngredientId).ToArray();
        var fromIngredients = model.ProductQuantities
            .SelectMany(z => z.ReplacedIngredients ?? Array.Empty<ReplacedIngredientModel>())
            .Select(x => x.FromIngredientId).ToArray();
        var toIngredients = model.ProductQuantities
            .SelectMany(z => z.ReplacedIngredients ?? Array.Empty<ReplacedIngredientModel>())
            .Select(x => x.ToIngredientId).ToArray();
        var ingredients = _context.Ingredients
            .Where(x => additionalIngredients.Concat(removedIngredients)
                .Concat(fromIngredients).Concat(toIngredients).Any(z => z == x.Id)).ToArray();

        Dictionary<AdditionalIngredientModel, int> AdditionalIngredientModels(Guid productId) =>
            (model.ProductQuantities.First(x => x.ProductId == productId).AdditionalIngredients ??
             Array.Empty<AdditionalIngredientModel>())
            .ToDictionary(x => x, x => x.Quantity);

        Dictionary<IngredientEntity, int> AdditionalIngredientEntities(Guid productId) =>
            ingredients.Where(x => AdditionalIngredientModels(productId).Any(z => z.Key.IngredientId == x.Id))
                .ToDictionary(x => x,
                    x => AdditionalIngredientModels(productId).First(z => z.Key.IngredientId == x.Id).Value);


        IngredientEntity[] RemovedIngredientEntities(Guid productId) => ingredients.Where(x =>
            (model.ProductQuantities.First(p => p.ProductId == productId)
                .RemovedIngredients ?? Array.Empty<RemovedIngredientModel>()).Any(z =>
                z.IngredientId == x.Id)).ToArray();


        Dictionary<IngredientEntity, IngredientEntity> ReplacedIngredients(Guid productId) =>
            (model.ProductQuantities.First(x => x.ProductId == productId).ReplacedIngredients ??
             Array.Empty<ReplacedIngredientModel>())
            .ToDictionary(x => ingredients.First(z => z.Id == x.FromIngredientId),
                x => ingredients.First(z => z.Id == x.ToIngredientId));

        return products.Select(x =>
            ToOrderedProduct(x, AdditionalIngredientEntities(x.Id), RemovedIngredientEntities(x.Id),
                ReplacedIngredients(x.Id))).ToArray();
    }

    private double ToPrice(OrderedProductEntity entity)
    {
        var sum = entity.OrderedProductOrderedIngredients.Sum(x =>
            entity.PizzaType is null || x.IsIngredientRemoved
                ? 0
                : x.ReplacedIngredient is not null
                    ? x.ReplacedIngredient.Prices[(int)entity.PizzaType] * x.Quantity
                    : x.OrderedIngredient.Prices[(int)entity.PizzaType] * x.Quantity);

        return entity.Price + sum;
    }

    private OrderedProductEntity ToOrderedProduct(ProductEntity entity,
        Dictionary<IngredientEntity, int> additionalIngredients,
        IngredientEntity[] removedIngredients,
        Dictionary<IngredientEntity, IngredientEntity> replacedIngredients)
    {
        var orderedProductOrderedIngredients = entity.Ingredients
            .Where(x => removedIngredients.All(z => z.Id != x.Id))
            .Where(x => replacedIngredients.All(z => z.Key.Id != x.Id))
            .Select(x => new OrderedProductOrderedIngredientEntity
            {
                Quantity = 1,
                OrderedIngredient = ToOrderedIngredient(x)
            }).Concat(additionalIngredients.Select(x => new OrderedProductOrderedIngredientEntity
            {
                Quantity = x.Value,
                IsAdditionalIngredient = true,
                OrderedIngredient = ToOrderedIngredient(x.Key)
            })).Concat(removedIngredients.Select(x => new OrderedProductOrderedIngredientEntity
            {
                Quantity = 1,
                IsIngredientRemoved = true,
                OrderedIngredient = ToOrderedIngredient(x)
            })).Concat(replacedIngredients.Select(x => new OrderedProductOrderedIngredientEntity
            {
                Quantity = 1,
                ReplacedIngredient = ToOrderedIngredient(x.Value),
                OrderedIngredient = ToOrderedIngredient(x.Key)
            })).ToArray();

        return new OrderedProductEntity
        {
            ProductId = entity.Id,
            PizzaType = entity.PizzaType,
            Name = entity.Name,
            Price = entity.Price,
            Description = entity.Description,
            ProductType = entity.ProductType,
            OrderedProductOrderedIngredients = orderedProductOrderedIngredients
        };
    }

    private OrderedIngredientEntity ToOrderedIngredient(IngredientEntity entity)
    {
        return new OrderedIngredientEntity
        {
            Name = entity.Name,
            PriceLarge = entity.PriceLarge,
            PriceMedium = entity.PriceMedium,
            PriceSmall = entity.PriceSmall,
            IngredientId = entity.Id,
        };
    }
}