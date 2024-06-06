using GoogleMaps.LocationServices;
using MikietaApi.Models;
using NetTopologySuite.Geometries;

namespace MikietaApi.Services;

interface IDeliveryService
{
    bool CheckDistance(DeliveryModel model);
}

public class DeliveryService : IDeliveryService
{
    private readonly GoogleLocationService _googleLocationService;

    //Approximate conversion factor
    private const double ConversionFactor = 111.32;
    private const double MaxDistanceInKm = 10;

    public DeliveryService(GoogleLocationService googleLocationService)
    {
        _googleLocationService = googleLocationService;
    }

    public bool CheckDistance(DeliveryModel model)
    {
        var target = ToPoint(model);

        var current = ToPoint(new DeliveryModel
        {
            City = "Czerwionka-Leszczyny",
            HomeNumber = "5",
            Street = "Adolfa Pojdy"
        });

        var distanceInKm = current.Distance(target) * ConversionFactor;

        return distanceInKm <= MaxDistanceInKm;
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