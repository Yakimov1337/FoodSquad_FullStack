using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSquad_API.Models.Entity
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("MenuItem")]
        public long MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
