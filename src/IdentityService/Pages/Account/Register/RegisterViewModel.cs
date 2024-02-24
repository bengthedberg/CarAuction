using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Register;

public class RegisterViewModel
{
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string FullName { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string? Button { get; set; } = string.Empty;

}
