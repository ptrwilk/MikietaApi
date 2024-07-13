using GoogleMaps.LocationServices;
using MikietaApi.Services;

namespace MikietaApi.Tests.Services;

public class DeliveryServiceTest
{
    [TestCase(1d, 2, ExpectedResult = 2)]
    [TestCase(1.1d, 2, ExpectedResult = 2)]
    [TestCase(1.99d, 2, ExpectedResult = 2)]
    [TestCase(0.9d, 2, ExpectedResult = 0)]
    [TestCase(2.9d, 4, ExpectedResult = 8)]
    [TestCase(2.9d, 0, ExpectedResult = 0)]
    public decimal CalculateDeliveryPrice(double distance, decimal deliveryPrice)
    {
        var service = new DeliveryService(new DataContextMock(), new GoogleLocationService());

        return Helpers.InvokePrivateMethod<decimal>(service, "CalculateDeliveryPrice", distance, deliveryPrice);;
    }
}