using Microsoft.Extensions.Logging;
using Wookiee.Model.Entities;
using Wookiee.Repository.Interface;
using Wookiee.Service.ImageManager;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.Book;
using Wookiee.Utility;
using Wookiee.Utility.Response;
using Wookiee.Service.Mapper.book;

namespace Wookiee.Service.Implementation;

public class BookService: IBookService
{
    #region field

    private readonly ILogger<BookService> _logger;
    private readonly IBookRepository _bookRepository;
    private readonly IHelper _helper;
    private readonly IUserRepository _userRepository;
    private const string DarthVader = "Darth Vader";
    private readonly IAmImageManager _amImageManager;

    #endregion

    #region constructor

    public BookService(ILogger<BookService> logger, IBookRepository bookRepository, IHelper helper, IUserRepository userRepository, IAmImageManager amImageManager)
    {
        _logger = logger;
        _bookRepository = bookRepository;
        _helper = helper;
        _userRepository = userRepository;
        _amImageManager = amImageManager;
    }

    #endregion

    #region implementation

    public async Task<ResponseObject<BookInfoDto>> Create(AddBookDto add)
    {
        try
        {
            var userId = _helper.GetLoggedId();
            if (string.IsNullOrWhiteSpace(userId))
                return await MapToBookResponseObject.ToBookInfoDto(null, false, "User not authenticated", null, _amImageManager);
            
            var isDarthVader = await IsDarthVader(userId);
            var bookId = await _bookRepository.CreateBook(new Book
            {
                IsPublished = !isDarthVader,
                Author = await _userRepository.FindById(userId),
                Description = add.Description,
                Image = add.Image != null ? await _amImageManager.UploadImage(add.Image) : null,
                Price = add.Price,
                Title = add.Title,
            });
            var book = await _bookRepository.ReadBook(bookId);
            return book == null 
                ? await MapToBookResponseObject.ToBookInfoDto(null, false, "Book not found", null, _amImageManager) 
                : await MapToBookResponseObject.ToBookInfoDto(book, true, null, null, _amImageManager);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return await MapToBookResponseObject.ToBookInfoDto(null, false, e.Message, e, _amImageManager);
        }
    }

    public async Task<ResponseObject<BookInfoDto>> Read(int id)
    {
        try
        {
            var book = await _bookRepository.ReadBook(id);
            return book == null 
                ? await MapToBookResponseObject.ToBookInfoDto(null, false, "Book not found", null, _amImageManager)
                : await MapToBookResponseObject.ToBookInfoDto(book, true, null, null, _amImageManager);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return await MapToBookResponseObject.ToBookInfoDto(null, false, e.Message, e, _amImageManager);
        }
    }

    public async Task<ResponseObject<BookInfoDto>> Update(UpdateBookDto update)
    {
        try
        {
            var book = await _bookRepository.ReadBook(update.Id);
            if (book == null)
                return await MapToBookResponseObject.ToBookInfoDto(null, false, "Book not found", null, _amImageManager);

            if (!book.Author!.Id.Equals(_helper.GetLoggedId()))
                return await MapToBookResponseObject.ToBookInfoDto(null, false, "This book is not owned by you", null, _amImageManager);

            // book.Image = _helper.ImageToBase64(update.Image);
            book.Price = update.Price;
            book.Title = update.Title;
            book.Description = update.Description;
            book.Id = update.Id;

            await _bookRepository.UpdateBook(book);

            return await Read(book.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return await MapToBookResponseObject.ToBookInfoDto(null, false, e.Message, e, _amImageManager);
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> Delete(int id)
    {
        try
        {
            var book = await _bookRepository.ReadBook(id);
            if (book == null)
                return MapToBookResponseObject.ToListBookInfoDto(null, false,
                    "Book not found", null);

            if (!book.Author!.Id.Equals(_helper.GetLoggedId()))
            {
                return MapToBookResponseObject.ToListBookInfoDto(null, false,
                    "This book is not owned by you", null);
            }

            await _bookRepository.DeleteBook(id);
            return await ReadList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return MapToBookResponseObject.ToListBookInfoDto(null, false,
                e.Message, e);
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> ReadList()
    {
        try
        {
            var response = await _bookRepository.ReadList();
            return MapToBookResponseObject.ToListBookInfoDto(response, true,
                null, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return MapToBookResponseObject.ToListBookInfoDto(null, false,
                e.Message, e);
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchAuthor(string authorPseudonym)
    {
        try
        {
            var authorIds = await _userRepository.FindByAuthorName(authorPseudonym);
            if (authorIds == null)
                return MapToBookResponseObject.ToListBookInfoDto(null, true,
                    "No result", null);

            var books = await _bookRepository.SearchAuthor(authorIds);
            return MapToBookResponseObject.ToListBookInfoDto(books, true,
                null, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return MapToBookResponseObject.ToListBookInfoDto(null, false,
                e.Message, e);
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchTitle(string title)
    {
        try
        {
            var books = await _bookRepository.SearchTitle(title);
            return MapToBookResponseObject.ToListBookInfoDto(books, true,
                null, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return MapToBookResponseObject.ToListBookInfoDto(null, false,
                e.Message, e);
        }
    }

    public async Task<ResponseObject<List<BookInfoDto>?>> SearchDescription(string description)
    {
        try
        {
            var books = await _bookRepository.SearchTitle(description);
            return MapToBookResponseObject.ToListBookInfoDto(books, true,
                null, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return MapToBookResponseObject.ToListBookInfoDto(null, false,
                e.Message, e);
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