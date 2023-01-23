using System.ComponentModel.DataAnnotations;

namespace Wookiee.WebAppApi.PostData.Book
{
    public class Update
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
