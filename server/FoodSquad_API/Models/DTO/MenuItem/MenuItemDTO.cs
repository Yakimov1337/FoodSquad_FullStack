using FoodSquad_API.Models.Entity;
using Newtonsoft.Json;

namespace FoodSquad_API.Models.DTO.MenuItem
{
    public class MenuItemDTO
    {
        [JsonIgnore] // Exclude from serialization
        public long Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool DefaultItem { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public int SalesCount { get; set; }
        public long ReviewCount { get; set; }
        public double AverageRating { get; set; }

        // Constructor to map from the MenuItem entity
        public MenuItemDTO(FoodSquad_API.Models.Entity.MenuItem menuItem, int salesCount, long reviewCount, double averageRating)
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

        // Default constructor
        public MenuItemDTO() { }
    }
}
