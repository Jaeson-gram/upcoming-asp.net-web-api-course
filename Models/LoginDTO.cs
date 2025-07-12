using System.ComponentModel.DataAnnotations;

namespace WebAPI2.Models;

public class LoginDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Policy { get; set; }

}