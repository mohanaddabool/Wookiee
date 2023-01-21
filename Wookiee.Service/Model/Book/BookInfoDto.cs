namespace Wookiee.Service.Model.Book;

public class BookInfoDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int BookId { get; set; }
    public string? AuthorName { get; set; }
}