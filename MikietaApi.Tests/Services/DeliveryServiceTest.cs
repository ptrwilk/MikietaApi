using GoogleMaps.LocationServices;
using MikietaApi.Services;

namespace MikietaApi.Tests.Services;

public class DeliveryServiceTest
{
    [TestCase(1d, 2d, ExpectedResult = 2d)]
    [TestCase(1.1d, 2d, ExpectedResult = 2d)]
    [TestCase(1.99d, 2d, ExpectedResult = 2d)]
    [TestCase(0.9d, 2d, ExpectedResult = 0d)]
    [TestCase(2.9d, 4d, ExpectedResult = 8d)]
    [TestCase(2.9d, 0d, ExpectedResult = 0d)]
    public double CalculateDeliveryPrice(double distance, double deliveryPrice)
    {
        var service = new DeliveryService(new DataContextMock(), new GoogleLocationService());
        
        return Helpers.InvokePrivateMethod<double>(service, "CalculateDeliveryPrice", distance, deliveryPrice);;
    }
}