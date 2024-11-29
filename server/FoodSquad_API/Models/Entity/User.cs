using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodSquad_API.Models.Enums;

namespace FoodSquad_API.Models.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Default Name";

        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(256)")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public UserRole Role { get; set; } = UserRole.Normal;

        [Required]
        public string ImageUrl { get; set; } = "https://example.com/default-avatar.png";

        [Required]
        public string PhoneNumber { get; set; } = "000-000-0000";

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public void AddToken(Token token)
        {
            Tokens.Add(token);
            token.User = this;
        }

        public void RemoveToken(Token token)
        {
            Tokens.Remove(token);
            token.User = null;
        }
    }
}
