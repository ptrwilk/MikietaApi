namespace MikietaApi.Models;

public class AddressModel
{
    public string Address { get; }
    
    public AddressModel(HttpContext context)
    {
        Address = $"{context.Request.Scheme}://{context.Request.Host}";
    }

    public override string ToString()
    {
        return Address;
    }
}