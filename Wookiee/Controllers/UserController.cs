using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.User;
using Wookiee.WebAppApi.PostData.User;

namespace Wookiee.WebAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        #region fields

        private readonly IUserService _userService;

        #endregion

        #region constructor

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region methods

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] Register registerUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _userService.Register(new RegisterDto
            {
                Password = registerUser.Password,
                UserName = registerUser.FirstName + " " + registerUser.LastName,
                FirstName = registerUser.FirstName!,
                LastName = registerUser.LastName!,
                Email = registerUser.Email,
                AuthorPseudonym = registerUser.AuthorPseudonym!,
            });

            return response.IsSuccess ?  new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] Login login)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _userService.Login(new LoginDto
            {
                Password = login.Password,
                Email = login.Email,
                RememberMe = login.RememberMe,
            });

            return response.IsSuccess ? new JsonResult(response.Result) : new JsonResult(response.ErrorMessage);
        }

        #endregion

    }
}
