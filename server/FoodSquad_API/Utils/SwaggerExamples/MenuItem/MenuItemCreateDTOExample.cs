using FoodSquad_API.Models.DTO.MenuItem;
using Swashbuckle.AspNetCore.Filters;

public class MenuItemCreateDTOExample : IExamplesProvider<MenuItemCreateDTO>
{
    public MenuItemCreateDTO GetExamples()
    {
        return new MenuItemCreateDTO
        {
            Title = "Cheeseburger",
            Description = "A delicious cheeseburger with fresh lettuce and tomato.",
            ImageUrl = "https://www.tastingtable.com/img/gallery/what-makes-restaurant-burgers-taste-different-from-homemade-burgers-upgrade/l-intro-1662064407.jpg",
            Price = 10.99,
            Category = "BURGER"
        };
    }
}
