using GoogleMaps.LocationServices;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using NetTopologySuite.Geometries;

namespace MikietaApi.Services;

public interface IDeliveryService
{
    DeliveryResponseModel CheckDistance(DeliveryModel model);
}

public class DeliveryService : IDeliveryService
{
    private readonly DataContext _context;
    private readonly GoogleLocationService _googleLocationService;

    //Approximate conversion factor
    private const double ConversionFactor = 111.32;
    private const double MaxDistanceInKm = 10;

    public DeliveryService(
        DataContext context,
        GoogleLocationService googleLocationService)
    {
        _context = context;
        _googleLocationService = googleLocationService;
    }

    public DeliveryResponseModel CheckDistance(DeliveryModel model)
    {
        try
        {
            var target = ToPoint(model);

            var current = ToPoint(new DeliveryModel
            {
                City = "Czerwionka-Leszczyny",
                HomeNumber = "5",
                Street = "Adolfa Pojdy"
            });

            var distanceInKm = current.Distance(target) * ConversionFactor;

            var isAtDistance = distanceInKm <= MaxDistanceInKm;

            if (!isAtDistance)
            {
                throw new DeliveryCheckException(DeliveryCheckErrorType.OutOfDeliveryRange,
                    "Out of delivery range.");
            }

            var deliveryPrice = _context.GetValue<double?>(SettingEntity.DeliveryPrice) ?? 0;

            return new DeliveryResponseModel
            {
                ErrorType = null,
                HasError = false,
                ErrorMessage = null,
                DeliveryPrice = CalculateDeliveryPrice(distanceInKm, deliveryPrice)
            };
        }
        catch (DeliveryCheckException ex)
        {
            return new DeliveryResponseModel
            {
                ErrorType = ex.Error.ErrorType,
                ErrorMessage = ex.Error.Message,
                HasError = true
            };
        }
    }

    private double CalculateDeliveryPrice(double distance, double deliveryPrice)
    {
        return (int)distance * deliveryPrice;
    }

    private Point ToPoint(DeliveryModel model)
    {
        var location = _googleLocationService.GetLatLongFromAddress(new AddressData
        {
            Address = $"{model.Street} {model.HomeNumber}",
            City = model.City,
        });

        if (location is null)
        {
            throw new DeliveryCheckException(DeliveryCheckErrorType.LocationNotFound, "Location not found.");
        }

        return new Point(location.Longitude, location.Latitude);
    }
}