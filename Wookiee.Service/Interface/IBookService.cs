using Wookiee.Service.Model.Book;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Interface;

public interface IBookService
{
    Task<ResponseObject<BookInfoDto>> Create(AddBookDto add);
    Task<ResponseObject<BookInfoDto>> Read(int id);
    Task<ResponseObject<BookInfoDto>> Update(UpdateBookDto update);
    Task<ResponseObject<List<BookInfoDto>?>> Delete(int id);
    Task<ResponseObject<List<BookInfoDto>?>> ReadList();
    Task<ResponseObject<List<BookInfoDto>?>> SearchAuthor(string authorPseudonym);
    Task<ResponseObject<List<BookInfoDto>?>> SearchTitle(string title);
    Task<ResponseObject<List<BookInfoDto>?>> SearchDescription(string description);
}