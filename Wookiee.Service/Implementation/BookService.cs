using Microsoft.Extensions.Logging;
using Wookiee.Model.Entities;
using Wookiee.Repository.Interface;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.Book;
using Wookiee.Utility;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Implementation;

public class BookService: IBookService
{
    #region field

    private readonly ILogger<BookService> _logger;
    private readonly IBookRepository _bookRepository;
    private readonly IHelper _helper;
    private readonly IUserRepository _userRepository;
    private const string DarthVader = "Darth Vader";

    #endregion

    #region constructor

    public BookService(ILogger<BookService> logger, IBookRepository bookRepository, IHelper helper, IUserRepository userRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
        _helper = helper;
        _userRepository = userRepository;
    }

    #endregion

    #region implementation

    public async Task<ResponseObject<BookInfoDto>> Create(AddBookDto add)
    {
        try
        {
            var userId = _helper.GetLoggedId();
            if (string.IsNullOrWhiteSpace(userId))
                return new ResponseObject<BookInfoDto>
                {
                    Exception = null,
                    ErrorMessage = "User not authenticated",
                    IsSuccess = false,
                    Result = null
                };

            var isDarthVader = await IsDarthVader(userId);

            var bookId = await _bookRepository.CreateBook(new Book
            {
                IsPublished = !isDarthVader,
                Author = await _userRepository.FindById(userId),
                Description = add.Description,
                Image = add.Image != null ? _helper.ReadImage(add.Image) : null,
                Price = add.Price,
                Title = add.Title,
            });
            var book = await _bookRepository.ReadBook(bookId);
            if (book == null)
                return new ResponseObject<BookInfoDto>
                {
                    Exception = null,
                    ErrorMessage = "Book not found",
                    IsSuccess = false,
                    Result = null
                };

            return new ResponseObject<BookInfoDto>
            {
                Result = new BookInfoDto
                {
                    Description = book.Description,
                    Image = book.Image,
                    Price = book.Price,
                    Title = book.Title,
                    AuthorName = book.Author?.AuthorPseudonym,
                    BookId = book.Id,
                },
                ErrorMessage = null,
                Exception = null,
                IsSuccess = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<BookInfoDto>
            {
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
                Result = null,
            };
        }
    }

    public async Task<ResponseObject<BookInfoDto>> Read(int id)
    {
        try
        {
            var book = await _bookRepository.ReadBook(id);
            if (book == null)
                return new ResponseObject<BookInfoDto>
                {
                    Exception = null,
                    ErrorMessage = "Book not found",
                    IsSuccess = false,
                    Result = null
                };

            return new ResponseObject<BookInfoDto>
            {
                Result = new BookInfoDto
                {
                    Description = book.Description,
                    Image = book.Image,
                    Price = book.Price,
                    Title = book.Title,
                    AuthorName = book.Author?.AuthorPseudonym,
                    BookId = book.Id
                },
                Exception = null,
                ErrorMessage = null,
                IsSuccess = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<BookInfoDto>
            {
                Result = null,
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
            };
        }
    }

    public async Task<ResponseObject<BookInfoDto>> Update(UpdateBookDto update)
    {
        try
        {
            var book = await _bookRepository.ReadBook(update.Id);
            if (book == null)
                return new ResponseObject<BookInfoDto> { Exception = null, ErrorMessage = "Book not found", IsSuccess = false, Result = null };

            book.Image = _helper.ReadImage(update.Image);
            book.Price = update.Price;
            book.Title = update.Title;
            book.Description = update.Description;
            book.Id = update.Id;

            _bookRepository.UpdateBook(book);

            return await Read(book.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<BookInfoDto>
            {
                Result = null,
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
            };
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>>> Delete(int id)
    {
        try
        {
            var book = await _bookRepository.ReadBook(id);
            if (book == null)
                return new ResponseObject<List<BookInfoDto>>
                {
                    Exception = null,
                    ErrorMessage = "Book not found",
                    IsSuccess = false,
                    Result = null
                };

            if (!book.Author!.Id.Equals(_helper.GetLoggedId()))
            {
                return new ResponseObject<List<BookInfoDto>>
                {
                    ErrorMessage = "This book is not owned by you",
                    IsSuccess = false,
                    Exception = null,
                    Result = null,
                };
            }

            _bookRepository.DeleteBook(id);
            return await ReadList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<List<BookInfoDto>>
            {
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
                Result = null,
            };
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>>> ReadList()
    {
        try
        {
            var response = await _bookRepository.ReadList();
            return new ResponseObject<List<BookInfoDto>>
            {
                Result = response?.Select(item => new BookInfoDto
                {
                    Price = item.Price,
                    Title = item.Title,
                    AuthorName = item.Author?.AuthorPseudonym,
                }).ToList(),
                Exception = null,
                ErrorMessage = null,
                IsSuccess = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<List<BookInfoDto>>
            {
                Result = null,
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
            };
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchAuthor(string authorPseudonym)
    {
        try
        {
            var authorIds = await _userRepository.FindByAuthorName(authorPseudonym);
            if (authorIds == null)
            {
                return new ResponseObject<List<BookInfoDto>?>
                {
                    Exception = null,
                    ErrorMessage = null,
                    IsSuccess = true,
                    Result = null,
                };
            }

            var books = await _bookRepository.SearchAuthor(authorIds);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Exception = null,
                ErrorMessage = null,
                IsSuccess = true,
                Result = books?.Select(item => new BookInfoDto
                {
                    Price = item.Price,
                    Title = item.Title,
                    AuthorName = item.Author?.AuthorPseudonym,
                }).ToList(),
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = true,
                Result = null,
            };
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchTitle(string title)
    {
        try
        {
            var books = await _bookRepository.SearchTitle(title);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Exception = null,
                ErrorMessage = null,
                IsSuccess = true,
                Result = books?.Select(b => new BookInfoDto
                {
                    AuthorName = b.Author?.AuthorPseudonym,
                    Title = b.Title,
                    Price = b.Price,
                }).ToList()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Result = null,
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
            };
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchDescription(string description)
    {
        try
        {
            var books = await _bookRepository.SearchTitle(description);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Exception = null,
                ErrorMessage = null,
                IsSuccess = true,
                Result = books?.Select(b => new BookInfoDto
                {
                    AuthorName = b.Author?.AuthorPseudonym,
                    Title = b.Title,
                    Price = b.Price,
                }).ToList()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new ResponseObject<List<BookInfoDto>?>
            {
                Result = null,
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
            };
        }
    }

    #endregion

    #region private methods

    private async Task<bool> IsDarthVader(string id)
    {
        var user = await _userRepository.FindById(id);
        if (user == null) throw new Exception("User not found");

        return user.AuthorPseudonym.Equals(DarthVader, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

}