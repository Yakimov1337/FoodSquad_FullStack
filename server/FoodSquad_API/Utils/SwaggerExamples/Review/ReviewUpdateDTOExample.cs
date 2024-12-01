using FoodSquad_API.Models.DTO.Review;
using Swashbuckle.AspNetCore.Filters;

public class ReviewUpdateDTOExample : IExamplesProvider<ReviewUpdateDTO>
{
    public ReviewUpdateDTO GetExamples()
    {
        return new ReviewUpdateDTO
        {
            Comment = "The fries were great, but the burger was a bit too salty.",
            Rating = 4
        };
    }
}
