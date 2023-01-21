using System.ComponentModel.DataAnnotations;

namespace Wookiee.Service.Model.User;

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string AuthorPseudonym { get; set; } = string.Empty;
}