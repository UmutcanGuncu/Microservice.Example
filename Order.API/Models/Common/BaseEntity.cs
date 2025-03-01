namespace Order.API.Models.Common;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
}