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
    private readonly ConfigurationOptions _configurationOptions;

    public SettingService(DataContext context, ConfigurationOptions configurationOptions)
    {
        _context = context;
        _configurationOptions = configurationOptions;
    }

    public SettingModel Get()
    {
        var settings = _context.Settings.ToArray();

        var openingHours = SettingEntity.OpensFrom.Select((key, index) => new SettingHoursModel
        {
            From = DataContext.GetValue<TimeSpan>(settings, key),
            To = DataContext.GetValue<TimeSpan>(settings, SettingEntity.OpensTo[index])
        }).ToArray();

        var deliveryHours = SettingEntity.DeliveriesFrom.Select((key, index) => new SettingHoursModel
        {
            From = DataContext.GetValue<TimeSpan>(settings, key),
            To = DataContext.GetValue<TimeSpan>(settings, SettingEntity.DeliveriesTo[index])
        }).ToArray();

        return new SettingModel
        {
            Street = DataContext.GetValue<string?>(settings, SettingEntity.Street),
            City = DataContext.GetValue<string?>(settings, SettingEntity.City),
            ZipCode = DataContext.GetValue<string?>(settings, SettingEntity.ZipCode),
            Phone = DataContext.GetValue<string?>(settings, SettingEntity.Phone),
            Facebook = DataContext.GetValue<string?>(settings, SettingEntity.Facebook),
            DeliveryPrice = DataContext.GetValue<double?>(settings, SettingEntity.DeliveryPrice),
            DeliveryRange = DataContext.GetValue<double?>(settings, SettingEntity.DeliveryRange),
            Email = DataContext.GetValue<string?>(settings, SettingEntity.Email),
            OpeningHours = openingHours,
            DeliveryHours = deliveryHours,
            AdminWebsiteUrl = _configurationOptions.AdminWebsiteUrl
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
        Update(settings, SettingEntity.Email, model.Email);

        for (var i = 0; i < SettingEntity.OpensFrom.Length; i++)
        {
            Update(settings, SettingEntity.OpensFrom[i], model.OpeningHours[i].From.ToString());
            Update(settings, SettingEntity.OpensTo[i], model.OpeningHours[i].To.ToString());

            Update(settings, SettingEntity.DeliveriesFrom[i], model.DeliveryHours[i].From.ToString());
            Update(settings, SettingEntity.DeliveriesTo[i], model.DeliveryHours[i].To.ToString());
        }

        _context.SaveChanges();

        return model;
    }

    private void Update(SettingEntity[] settings, string key, string? value)
    {
        var setting = settings.First(x => x.Key == key);
        setting.Value = value;
    }
}