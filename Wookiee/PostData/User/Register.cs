using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Wookiee.WebAppApi.PostData.User
{
    public class Register
    {
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Author pseudonym is required")]
        public string? AuthorPseudonym { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
