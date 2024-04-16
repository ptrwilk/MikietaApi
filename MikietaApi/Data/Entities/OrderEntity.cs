using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class OrderEntity
{
    [Key]
    public int Id { get; set; }
    public int Number { get; set; }
    public ICollection<ProductEntity>? Products { get; set; }
    public DateTime? DeliveryTiming { get; set; }
    public bool? DeliveryRightAway { get; set; }
    public DeliveryMethodType DeliveryMethod { get; set; }
    public string? Comments { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Nip { get; set; }
    public bool? ProcessingPersonalDataByEmail { get; set; }
    public bool? ProcessingPersonalDataBySms { get; set; }
}