using Wookiee.Service.Model.Book;
using Wookiee.Utility.Response;
using Wookiee.WebAppApi.PostData.Book;

namespace Wookiee.WebAppApi.Test.Model.Book;

public static class Mapper
{
    public static Create ToCreate()
    {
        return new Create
        {
            Description = "Test description",
            Price = 50.50M,
            Title = "Test title",
            Image = null,
        };
    }

    public static Update ToUpdate()
    {
        return new Update
        {
            Description = "Test description",
            Price = 50.50M,
            Title = "Test title",
            Id = 1,
            Image = null,
        };
    }

    public static ResponseObject<BookInfoDto> ToResponseObject()
    {
        return new ResponseObject<BookInfoDto>
        {
            Exception = null,
            ErrorMessage = null,
            IsSuccess = true,
            Result = new BookInfoDto
            {
                Description = "Test description",
                Price = 50.50M,
                Image = string.Empty,
                AuthorName = "Test author",
                BookId = 1,
                Title = "Test title",
            }
        };
    }

    public static ResponseObject<List<BookInfoDto>> ToListResponseObject()
    {
        return new ResponseObject<List<BookInfoDto>>
        {
            Exception = null,
            ErrorMessage = null,
            IsSuccess = true,
            Result = new List<BookInfoDto>
            {
                new BookInfoDto
                {
                    Description = "Test description",
                    Price = 50.50M,
                    Image = string.Empty,
                    AuthorName = "Test author",
                    BookId = 1,
                    Title = "Test title",
                }
            }
        };
    }
}