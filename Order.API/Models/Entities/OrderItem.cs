using Order.API.Models.Common;

namespace Order.API.Models.Entities;

public class OrderItem : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    public Order Order { get; set; }
}