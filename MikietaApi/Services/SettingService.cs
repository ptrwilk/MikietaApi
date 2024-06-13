using MikietaApi.Data;
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
            From = _context.GetValue<TimeSpan>(settings, key),
            To = _context.GetValue<TimeSpan>(settings, SettingEntity.OpensTo[index])
        }).ToArray();

        var deliveryHours = SettingEntity.DeliveriesFrom.Select((key, index) => new SettingHoursModel
        {
            From = _context.GetValue<TimeSpan>(settings, key),
            To = _context.GetValue<TimeSpan>(settings, SettingEntity.DeliveriesTo[index])
        }).ToArray();

        return new SettingModel
        {
            Street = _context.GetValue<string?>(settings, SettingEntity.Street),
            City = _context.GetValue<string?>(settings, SettingEntity.City),
            ZipCode = _context.GetValue<string?>(settings, SettingEntity.ZipCode),
            Phone = _context.GetValue<string?>(settings, SettingEntity.Phone),
            Facebook = _context.GetValue<string?>(settings, SettingEntity.Facebook),
            DeliveryPrice = _context.GetValue<double?>(settings, SettingEntity.DeliveryPrice),
            DeliveryRange = _context.GetValue<double?>(settings, SettingEntity.DeliveryRange),
            Email = _context.GetValue<string?>(settings, SettingEntity.Email),
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