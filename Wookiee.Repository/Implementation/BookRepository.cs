using Microsoft.EntityFrameworkCore;
using Wookiee.Model.Entities;
using Wookiee.Repository.Context;
using Wookiee.Repository.Interface;

namespace Wookiee.Repository.Implementation;

public class BookRepository: IBookRepository
{
    #region field

    private readonly WookieeContext _context;

    #endregion

    #region constructor

    public BookRepository(WookieeContext context)
    {
        _context = context;
    }

    #endregion

    #region implementation

    public async Task<int> CreateBook(Book create)
    {
        var book = await _context.Books.AddAsync(create);
        await _context.SaveChangesAsync();
        return book.Entity.Id;
    }

    public async Task DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        book!.IsPublished = false;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBook(Book update)
    {
        _context.Books.Update(update);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Book>?> ReadList()
    {
        return await _context.Books.Include(u => u.Author).Where(x => x.IsPublished).ToListAsync();
    }

    public async Task<Book?> ReadBook(int id)
    {
        return await _context.Books.Include(u => u.Author).FirstOrDefaultAsync(x => x.Id == id && x.IsPublished);
    }

    public async Task<List<Book>?> SearchTitle(string searchQuery)
    {
        return await _context.Books.Include(u => u.Author).Where(b => EF.Functions.Like(b.Title, $"%{searchQuery}%") && b.IsPublished).ToListAsync();
    }

    public async Task<List<Book>?> SearchDescription(string searchQuery)
    {
        return await _context.Books.Include(u => u.Author).Where(b => EF.Functions.Like(b.Description, $"%{searchQuery}%") && b.IsPublished).ToListAsync();
    }

    public async Task<List<Book>?> SearchAuthor(List<string> authorIds)
    {
        return await _context.Books.Include(u => u.Author).Where(b => authorIds.Contains(b.Author!.Id) && b.IsPublished).ToListAsync();
    }

    #endregion

}