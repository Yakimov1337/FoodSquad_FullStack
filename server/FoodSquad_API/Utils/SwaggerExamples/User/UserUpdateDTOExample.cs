using FoodSquad_API.Models.DTO.User;
using Swashbuckle.AspNetCore.Filters;

public class UserUpdateDTOExample : IExamplesProvider<UserUpdateDTO>
{
    public UserUpdateDTO GetExamples()
    {
        return new UserUpdateDTO
        {
            Name = "John Doe",
            Role = "Moderator",
            ImageUrl = "https://example.com/john-doe-avatar.png",
            PhoneNumber = "+1234567890"
        };
    }
}
