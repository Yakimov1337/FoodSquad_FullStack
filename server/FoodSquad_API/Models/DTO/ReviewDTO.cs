using System;
using System.ComponentModel.DataAnnotations;

namespace FoodSquad_API.Models.DTO
{
    public class ReviewDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Comment cannot be blank")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Menu Item ID is required")]
        public long MenuItemId { get; set; }

        public string UserEmail { get; set; }
        public string ImageUrl { get; set; }
    }
}
