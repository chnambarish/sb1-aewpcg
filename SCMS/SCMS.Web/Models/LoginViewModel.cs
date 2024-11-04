using System.ComponentModel.DataAnnotations;

namespace SCMS.Web.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}