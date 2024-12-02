using FoodSquad_API.Models.DTO.User;
using Swashbuckle.AspNetCore.Filters;

public class UserLoginDTOExample : IExamplesProvider<UserLoginDTO>
{
    public UserLoginDTO GetExamples()
    {
        return new UserLoginDTO
        {
            Email = "admin@example.com",
            Password = "123123",
        };
    }
}
