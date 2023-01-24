using System.Diagnostics.CodeAnalysis;

namespace Wookiee.Model.Entities;

public class Book
{
    public int Id { get; set; }
    [NotNull]
    public string? Description { get; set; }
    [NotNull]
    public string? Title { get; set; }
    [NotNull]
    public decimal Price { get; set; }
    [NotNull]
    public bool IsPublished { get; set; } = true;
    [NotNull]
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public virtual User? Author { get; set; }
    public virtual Image? Image { get; set; }
}