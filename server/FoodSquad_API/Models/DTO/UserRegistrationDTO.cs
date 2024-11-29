using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO
{
    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email should be valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Size must be between 6 and 20 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(20, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }
}
