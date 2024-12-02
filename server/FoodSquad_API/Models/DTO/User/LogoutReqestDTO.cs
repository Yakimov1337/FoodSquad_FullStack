using System.ComponentModel.DataAnnotations;

public class LogoutRequestDTO
{
    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}
