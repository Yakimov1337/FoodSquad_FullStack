using FoodSquad_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSquad_API.Models.Entity
{
    public class MenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; } = "https://example.com/default-menu-item-image.png";

        [Required]
        public bool DefaultItem { get; set; } = false;

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        public MenuItemCategory Category { get; set; } = MenuItemCategory.Other;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<Review> Reviews { get; set; } = new();

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
