namespace MikietaApi.Data.Entities;

public class OrderProductEntity
{
    public int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public int Quantity { get; set; }
}