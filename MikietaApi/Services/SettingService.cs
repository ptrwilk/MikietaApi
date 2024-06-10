﻿using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

interface ISettingService
{
    SettingModel Get();
    SettingModel Update(SettingModel model);
}

public class SettingService : ISettingService
{
    private readonly DataContext _context;

    public SettingService(DataContext context)
    {
        _context = context;
    }
    
    public SettingModel Get()
    {
        var settings = _context.Settings.ToArray();
        
        var openingHours = SettingEntity.OpensFrom.Select((key, index) => new SettingHoursModel
        {
            From = GetValue<TimeSpan>(settings, key),
            To = GetValue<TimeSpan>(settings, SettingEntity.OpensTo[index])
        }).ToArray();
        
        var deliveryHours = SettingEntity.DeliveriesFrom.Select((key, index) => new SettingHoursModel
        {
            From = GetValue<TimeSpan>(settings, key),
            To = GetValue<TimeSpan>(settings, SettingEntity.DeliveriesTo[index])
        }).ToArray() ;

        return new SettingModel
        {
            Street = GetValue<string?>(settings, SettingEntity.Street),
            City = GetValue<string?>(settings, SettingEntity.City),
            ZipCode = GetValue<string?>(settings, SettingEntity.ZipCode),
            Phone = GetValue<string?>(settings, SettingEntity.Phone),
            Facebook = GetValue<string?>(settings, SettingEntity.Facebook),
            DeliveryPrice = GetValue<double?>(settings, SettingEntity.DeliveryPrice),
            DeliveryRange = GetValue<double?>(settings, SettingEntity.DeliveryRange),
            OpeningHours = openingHours,
            DeliveryHours = deliveryHours
        };
    }

    public SettingModel Update(SettingModel model)
    {
        var settings = _context.Settings.ToArray();

        Update(settings, SettingEntity.Street, model.Street);
        Update(settings, SettingEntity.City, model.City);
        Update(settings, SettingEntity.ZipCode, model.ZipCode);
        Update(settings, SettingEntity.Phone, model.Phone);
        Update(settings, SettingEntity.Facebook, model.Facebook);
        Update(settings, SettingEntity.DeliveryPrice, model.DeliveryPrice.ToString());
        Update(settings, SettingEntity.DeliveryRange, model.DeliveryRange.ToString());

        for (var i = 0; i < SettingEntity.OpensFrom.Length; i++)
        {
            Update(settings, SettingEntity.OpensFrom[i], model.OpeningHours[i].From.ToString());
            Update(settings, SettingEntity.OpensTo[i], model.OpeningHours[i].To.ToString());
            
            Update(settings, SettingEntity.DeliveriesFrom[i], model.DeliveryHours[i].From.ToString());
            Update(settings, SettingEntity.DeliveriesFrom[i], model.DeliveryHours[i].To.ToString());
        }

        _context.SaveChanges();

        return model;
    }

    private T? GetValue<T>(SettingEntity[] settings, string key)
    {
        var value = settings.Single(x => x.Key == key).Value;

        if (string.IsNullOrEmpty(value))
        {
            return default;
        }
        
        if (typeof(T) == typeof(TimeSpan))
        {
            return (T)(object)TimeSpan.Parse(value ?? "");
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)value;
        }
        
        if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
        {
            return (T)(object)double.Parse(value);
        }

        throw new ArgumentException($"Case not specified for type {typeof(T)}");
    }

    private void Update(SettingEntity[] settings, string key, string? value)
    {
        var setting = settings.First(x => x.Key == key);
        setting.Value = value;
    }
}