using Microsoft.AspNetCore.Identity;

namespace Wookiee.Model.Entities;

public class User: IdentityUser
{
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public string AuthorPseudonym { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public virtual List<Book>? Books { get; set; }
    
}