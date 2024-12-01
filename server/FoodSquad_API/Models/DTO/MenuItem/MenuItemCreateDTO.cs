using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO.MenuItem
{
    public class MenuItemCreateDTO
    {
        [Required(ErrorMessage = "Title cannot be blank")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description cannot be blank")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image URL cannot be blank")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Price cannot be null")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Category cannot be null")]
        public string Category { get; set; }
    }
}
