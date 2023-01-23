using Wookiee.Model.Entities;

namespace Wookiee.Repository.Interface;

public interface IBookRepository
{
    Task<int> CreateBook(Book create);
    Task DeleteBook(int id);
    Task UpdateBook(Book update);
    Task<List<Book>?> ReadList();
    Task<Book?> ReadBook(int id);
    Task<List<Book>?> SearchTitle(string searchQuery);
    Task<List<Book>?> SearchDescription(string searchQuery);
    Task<List<Book>?> SearchAuthor(List<string> authorIds);
}