using System.ComponentModel.DataAnnotations;

namespace DahiliaCreations.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    // Encrypted (hashed) password
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [Display(Name = "Password")]
    public string PasswordHash { get; set; }

    public string Role { get; set; } // "Admin" or "Customer"

    public bool IsActive { get; set; } = true;

    // For forget password
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
}

