using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSquad_API.Models.Entity
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string AccessToken { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime AccessTokenExpiryDate { get; set; }

        [Required]
        public DateTime RefreshTokenExpiryDate { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
