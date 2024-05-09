﻿using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IOrderService
{
    int Order(OrderModel model);
}

public class OrderService : IOrderService
{
    private readonly DataContext _context;

    public OrderService(DataContext context)
    {
        _context = context;
    }
    
    public int Order(OrderModel model)
    {
        var products = _context.Products.Where(x => model.ProductQuantities.Select(g => g.ProductId).Any(z => x.Id == z)).ToArray();
        
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
        
        _context.SaveChanges();

        return entity.Number;
    }
}