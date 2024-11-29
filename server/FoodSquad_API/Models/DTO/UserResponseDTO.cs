namespace FoodSquad_API.Models.DTO
{
    using FoodSquad_API.Models.Entity;

    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public long OrdersCount { get; set; }

        public UserResponseDTO() { }

        public UserResponseDTO(User user)
        {
            Id = user.Id.ToString();
            Name = user.Name;
            Email = user.Email;
            Role = user.Role.ToString();
            ImageUrl = user.ImageUrl;
            PhoneNumber = user.PhoneNumber;
        }
    }
}
