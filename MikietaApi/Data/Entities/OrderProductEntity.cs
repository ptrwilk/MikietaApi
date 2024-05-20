namespace MikietaApi.Data.Entities;

public class OrderProductEntity
{
    public Guid OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public int Quantity { get; set; }
    public bool Ready { get; set; }
}