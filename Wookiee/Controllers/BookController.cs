using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.Book;
using Wookiee.WebAppApi.PostData.Book;

namespace Wookiee.WebAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        #region fields

        private readonly IBookService _bookService;

        #endregion

        #region constructors

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        #endregion
        
        #region methods

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromForm] Create request, IFormFile? image)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _bookService.Create(new AddBookDto
            {
                Description = request.Description,
                Image = image,
                Price = request.Price,
                Title = request.Title,
            });
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpPut]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromForm] Update request, IFormFile? image)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _bookService.Update(new UpdateBookDto
            {
                Description = request.Description,
                Image = image,
                Price = request.Price,
                Title = request.Title,
                Id = request.Id,
            });
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpDelete]
        [Authorize]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _bookService.Delete(id);
            return response.IsSuccess ? new JsonResult(response) : new JsonResult(response.ErrorMessage);
        }

        [HttpGet]
        [Authorize]
        [Route("get/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _bookService.Read(id);
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpGet]
        [Authorize]
        [Route("getlist")]
        public async Task<IActionResult> GetList()
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _bookService.ReadList();
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("search/author/{authorName}")]
        public async Task<IActionResult> SearchByAuthor(string authorName)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (string.IsNullOrWhiteSpace(authorName)) return BadRequest("Author is empty");
            var response = await _bookService.SearchAuthor(authorName);
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("search/title/{title}")]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is empty");
            var response = await _bookService.SearchTitle(title);
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("search/description/{description}")]
        public async Task<IActionResult> SearchByDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return BadRequest("Description is empty");
            var response = await _bookService.SearchDescription(description);
            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        #endregion

    }
}
