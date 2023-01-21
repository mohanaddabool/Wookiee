using System.ComponentModel.DataAnnotations;

namespace Wookiee.Service.Model.User;

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? AuthorPseudonym { get; set; }
}