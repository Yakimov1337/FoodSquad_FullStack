using FoodSquad_API.Models.DTO.Review;
using Swashbuckle.AspNetCore.Filters;

public class ReviewCreateDTOExample : IExamplesProvider<ReviewCreateDTO>
{
    public ReviewCreateDTO GetExamples()
    {
        return new ReviewCreateDTO
        {
            Comment = "The burger was fantastic, especially with the extra cheese.",
            Rating = 5,
            MenuItemId = 1
        };
    }
}
