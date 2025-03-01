namespace Order.API.ViewModels;

public class CreateOrderViewModel
{
    public Guid BuyerId { get; set; }
    public List<CreateOrderItemViewModel> OrderItems { get; set; }
}
