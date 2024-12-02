using FoodSquad_API.Models.DTO.User;
using Swashbuckle.AspNetCore.Filters;

public class UserRegistrationDTOExample : IExamplesProvider<UserRegistrationDTO>
{
    public UserRegistrationDTO GetExamples()
    {
        return new UserRegistrationDTO
        {
            Email = "newuser@example.com",
            Password = "123123",
            ConfirmPassword = "123123"
        };
    }
}
