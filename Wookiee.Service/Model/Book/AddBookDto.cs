using Microsoft.AspNetCore.Http;

namespace Wookiee.Service.Model.Book;

public class AddBookDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile? Image { get; set; }
}