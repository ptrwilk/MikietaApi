namespace MikietaApi.Data.Entities;

public class OrderOrderedProductEntity
{
    public Guid OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public Guid OrderedProductId { get; set; }
    public OrderedProductEntity OrderedProduct { get; set; } = null!;
    public int Quantity { get; set; }
    public bool Ready { get; set; }
}