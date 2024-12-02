using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO.User
{
    public class UserUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        public string Role { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; }

        [Phone(ErrorMessage = "Phone number is invalid")]
        public string PhoneNumber { get; set; }
    }
}
