using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class OrderEntity
{
    [Key]
    public int Id { get; set; }
    public int Number { get; set; }
    public ICollection<OrderProductEntity> OrderProducts { get; set; }
    public DateTime? DeliveryTiming { get; set; }
    public bool? DeliveryRightAway { get; set; }
    public DeliveryMethodType DeliveryMethod { get; set; }
    public string? Comments { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Nip { get; set; }
    public string? Street { get; set; }
    public string? HomeNumber { get; set; }
    public string? City { get; set; }
    public string? FlatNumber { get; set; }
    public string? Floor { get; set; }
    public bool? ProcessingPersonalDataByEmail { get; set; }
    public bool? ProcessingPersonalDataBySms { get; set; }
    public bool Paid { get; set; }
    public string? SessionId { get; set; }
    public DateTime? CreatedAt { get; set; }
}