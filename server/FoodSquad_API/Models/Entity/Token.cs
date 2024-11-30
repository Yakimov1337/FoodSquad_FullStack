using FoodSquad_API.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Token
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column(TypeName = "NVARCHAR(MAX)")] // Use NVARCHAR(MAX) instead of TEXT
    public string AccessToken { get; set; }

    [Required]
    [Column(TypeName = "NVARCHAR(MAX)")] // Use NVARCHAR(MAX) instead of TEXT
    public string RefreshToken { get; set; }

    [Required]
    public DateTime AccessTokenExpiryDate { get; set; }

    [Required]
    public DateTime RefreshTokenExpiryDate { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
}
