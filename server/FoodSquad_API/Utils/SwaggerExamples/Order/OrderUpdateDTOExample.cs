using FoodSquad_API.Models.DTO.Order;
using Swashbuckle.AspNetCore.Filters;

public class OrderUpdateDTOExample : IExamplesProvider<OrderUpdateDTO>
{
    public OrderUpdateDTO GetExamples()
    {
        return new OrderUpdateDTO
        {
            MenuItemQuantities = new Dictionary<long, int>
            {
                { 1, 3 },
                { 2, 2 }
            },
            Status = "COMPLETED",
            Paid = true
        };
    }
}
