using Order.API.Models.Common;
using Order.API.Models.Enums;

namespace Order.API.Models.Entities;

public class Order : BaseEntity
{
    public Guid BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}