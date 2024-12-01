using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO.MenuItem
{
    public class MenuItemUpdateDTO
    {
        [Required(ErrorMessage = "Title cannot be blank")]
        [SwaggerSchema(Description = "Title of the menu item.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description cannot be blank")]
        [SwaggerSchema(Description = "Detailed description of the menu item.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image URL cannot be blank")]
        [Url(ErrorMessage = "Invalid URL format")]
        [SwaggerSchema(Description = "Image URL of the menu item.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Price cannot be null")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
        [SwaggerSchema(Description = "Price of the menu item.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Category cannot be null")]
        [SwaggerSchema(Description = "Category of the menu item (e.g., BURGER, PIZZA, SALAD).")]
        public string Category { get; set; }
    }
}
