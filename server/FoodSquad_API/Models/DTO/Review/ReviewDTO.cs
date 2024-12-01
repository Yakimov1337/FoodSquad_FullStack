using System;

namespace FoodSquad_API.Models.DTO.Review
{
    public class ReviewDTO
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public long MenuItemId { get; set; }
        public string UserEmail { get; set; }
        public string ImageUrl { get; set; }
    }
}
