using Swashbuckle.AspNetCore.Filters;

namespace FoodSquad_API.Models.DTO.MenuItem
{
    public class MenuItemUpdateDTOExample : IExamplesProvider<MenuItemUpdateDTO>
    {
        public MenuItemUpdateDTO GetExamples()
        {
            return new MenuItemUpdateDTO
            {
                Title = "Veggie Burger",
                Description = "A delicious veggie burger with lettuce and tomato.",
                ImageUrl = "https://www.example.com/veggie-burger.jpg",
                Price = 8.99,
                Category = "BURGER"
            };
        }
    }
}
