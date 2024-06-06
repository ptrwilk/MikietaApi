namespace MikietaApi.Models;

public class DeliveryModel
{
    public string Street { get; set; } = null!;
    public string HomeNumber { get; set; } = null!;
    public string City { get; set; } = null!;
}

public class DeliveryCheckError
{
    public DeliveryCheckErrorType ErrorType { get; set; }
    public string Message { get; set; } = null!;
}

public enum DeliveryCheckErrorType
{
    LocationNotFound
}

public class DeliveryCheckException : Exception
{
    public DeliveryCheckError Error { get; }

    public DeliveryCheckException(DeliveryCheckErrorType type, string message)
    {
        Error = new DeliveryCheckError
        {
            ErrorType = type,
            Message = message
        };
    }
}