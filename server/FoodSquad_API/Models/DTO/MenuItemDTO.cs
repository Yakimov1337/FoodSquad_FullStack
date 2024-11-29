using FoodSquad_API.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO
{
    public class MenuItemDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Title cannot be blank")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description cannot be blank")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image URL cannot be blank")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; }

        public bool DefaultItem { get; set; }

        [Required(ErrorMessage = "Price cannot be null")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Category cannot be null")]
        public string Category { get; set; }

        public int SalesCount { get; set; }
        public long ReviewCount { get; set; }
        public double AverageRating { get; set; }

        public MenuItemDTO(MenuItem menuItem, int salesCount, long reviewCount, double averageRating)
        {
            Id = menuItem.Id;
            Title = menuItem.Title;
            Description = menuItem.Description;
            ImageUrl = menuItem.ImageUrl;
            DefaultItem = menuItem.DefaultItem; 
            Price = menuItem.Price;
            Category = menuItem.Category.ToString();
            SalesCount = salesCount;
            ReviewCount = reviewCount;
            AverageRating = averageRating;
        }

    }
}
