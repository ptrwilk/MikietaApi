﻿using Microsoft.AspNetCore.SignalR;
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
    public string SessionId { get; set; } = null!;
    public string Url { get; set; } = null!;
}

public interface IOrderService
{
    OrderResponseModel2 Order(OrderModel model);
    int OrderSuccess(string sessionId);
    AdminOrderModel[] GetAll();
    AdminProductModel[] Get(int orderId);
    AdminOrderModel GetSingle(int orderId);
    OrderStatusModel GetStatus(int orderId);
    AdminOrderModel Update(AdminOrderModel model);
    AdminProductModel UpdateProduct(int orderId, AdminProductModel model);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;
    private readonly StripeFacade _stripe;
    private readonly IConverter<OrderProductEntity, StripeRequestModel> _converter;
    private readonly IHubContext<MessageHub, IMessageHub> _hub;

    public OrderService(DataContext context, StripeFacade stripe,
        IConverter<OrderProductEntity, StripeRequestModel> converter,
        IHubContext<MessageHub, IMessageHub> hub)
    {
        _context = context;
        _stripe = stripe;
        _converter = converter;
        _hub = hub;
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
            DeliveryTiming = model.DeliveryTiming?.ToLocalTime() ?? DateTime.Now,
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

    public AdminOrderModel[] GetAll()
    {
        return _context.Orders.Include(x => x.OrderProducts)
            .ThenInclude(x => x.Product)
            .ToList()
            .Select(Convert).ToArray();
    }

    public AdminProductModel[] Get(int orderId)
    {
        return _context.OrderProducts.Include(x => x.Product)
            .Where(x => x.OrderId == orderId)
            .ToList()
            .Select(Convert)
            .ToArray();
    }

    public AdminOrderModel GetSingle(int orderId)
    {
        var entity = _context.Orders.Include(x => x.OrderProducts)
            .ThenInclude(x => x.Product)
            .First(x => x.Id == orderId);

        return Convert(entity);
    }

    public OrderStatusModel GetStatus(int orderId)
    {
        var entity = _context.Orders.First(x => x.Id == orderId);
        
        return new OrderStatusModel
        {
            Status = entity.Status,
            DeliveryAt = entity.DeliveryTiming
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

        _context.SaveChanges();

        if (canUpdate)
        {
            _hub.Clients(model.Id).OrderChanged();
        }

        return model;
    }

    public AdminProductModel UpdateProduct(int orderId, AdminProductModel model)
    {
        var entity = _context.OrderProducts
            .Single(x => x.OrderId == orderId && x.ProductId == model.Id);

        entity.Ready = model.Ready;

        _context.SaveChanges();

        return model;
    }

    private AdminProductModel Convert(OrderProductEntity entity)
    {
        return new AdminProductModel
        {
            Id = entity.Product.Id,
            Name = entity.Product.Name,
            Price = entity.Product.Price * entity.Quantity,
            Type = EnumConverter.Convert(entity.Product.ProductType),
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
            Address = $"{entity.City}",
            Cost = entity.OrderProducts.Where(z => z.OrderId == entity.Id).Sum(z => z.Product.Price * z.Quantity),
            Phone = entity.Phone,
            Number = entity.Number,
            Payed = entity.Paid,
            Status = entity.Status,
            OnSitePickup = entity.DeliveryMethod == DeliveryMethodType.TakeAway,
            TotalProducts = entity.OrderProducts.Count,
            CompletedProducts = entity.OrderProducts.Count(z => z.Ready),
            CreatedAt = entity.CreatedAt,
            DeliveryAt = entity.DeliveryTiming
        };
    }
}