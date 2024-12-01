using FoodSquad_API.Models.DTO.Order;
using Swashbuckle.AspNetCore.Filters;

public class OrderCreateDTOExample : IExamplesProvider<OrderCreateDTO>
{
    public OrderCreateDTO GetExamples()
    {
        return new OrderCreateDTO
        {
            MenuItemQuantities = new Dictionary<long, int>
            {
                { 1, 2 },
                { 2, 1 }
            },
            Status = "PENDING",
            CreatedOn = DateTime.UtcNow,
            Paid = false
        };
    }
}
