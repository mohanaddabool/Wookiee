using Wookiee.Model.Entities;
using Wookiee.Service.Model.Book;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Mapper.book;

public static class MapToBookResponseObject
{
    public static ResponseObject<BookInfoDto> ToBookInfoDto(Book? result, bool isSuccess, string? errorMessage, Exception? exception)
    {
        return new ResponseObject<BookInfoDto>
        {
            Exception = exception,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            Result = result != null ? new BookInfoDto
            {
                Image = result.Image,
                Description = result.Description,
                Price = result.Price,
                AuthorName = result.Author!.AuthorPseudonym,
                BookId = result.Id,
                Title = result.Title,
            } : null,
        };
    }

    public static ResponseObject<List<BookInfoDto>?> ToListBookInfoDto(List<Book>? result, bool isSuccess, string? errorMessage, Exception? exception)
    {
        return new ResponseObject<List<BookInfoDto>?>
        {
            Exception = exception,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            Result = result?.Select(b => new BookInfoDto
            {
                Title = b.Title,
                AuthorName = b.Author!.AuthorPseudonym,
                Price = b.Price,
            }).ToList(),
        };
    } 
}