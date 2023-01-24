using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Wookiee.Model.Entities;

public class Image
{
    public Guid ImageId { get; set; }
    [NotNull]
    public string? Extension { get; set; }

    [NotNull]
    public virtual Book? Book { get; set; }
    public int BookId { get; set; }
}