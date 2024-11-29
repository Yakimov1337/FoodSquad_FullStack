namespace FoodSquad_API.Models.DTO
{
    public class LogoutRequestDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
