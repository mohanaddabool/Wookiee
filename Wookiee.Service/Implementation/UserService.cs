using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Wookiee.Model.Entities;
using Wookiee.Repository.Interface;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.User;
using Wookiee.Utility;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Implementation;

public class UserService: IUserService
{
    #region field

    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IHelper _helper;

    #endregion

    #region constructor

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IHelper helper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _helper = helper;
    }

    #endregion

    #region implementation

    public async Task<ResponseObject<string>> Register(RegisterDto user)
    {
        try
        {
            var identityResult = await _userRepository.Register(new User
            {
                UserName = user.UserName,
                Email = user.Email,
                AuthorPseudonym = user.AuthorPseudonym,
                FirstName = user.FirstName,
                LastName = user.LastName,
            },
                user.Password!);

            if (identityResult.Succeeded)
            {
                return new ResponseObject<string>
                {
                    Result = "User is created",
                    Exception = null,
                    ErrorMessage = null,
                    IsSuccess = identityResult.Succeeded,
                };
            }
            return new ResponseObject<string>
            {
                Exception = null,
                ErrorMessage = identityResult.Errors.Select(x => x.Description),
                Result = "User is not created please check the error message",
                IsSuccess = identityResult.Succeeded,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Register user going wrong");
            return new ResponseObject<string>
            {
                Result = e.Message,
                ErrorMessage = e.Message,
                IsSuccess = false,
                Exception = e
            };

        }
    }

    public async Task<ResponseObject<string>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userRepository.FindByEmail(loginDto.Email!);
            if (user == null)
            {
                return new ResponseObject<string>
                {
                    Exception = null,
                    ErrorMessage = "Login failed, please check email or password",
                    IsSuccess = false,
                    Result = "Please check your email address",
                };
            }
            var signInResult = await _userRepository.Login(user.UserName!, loginDto.Password!, loginDto.RememberMe);
            if (!signInResult.Succeeded)
                return new ResponseObject<string>
                {
                    Exception = null,
                    ErrorMessage = "Login failed, please check email or password",
                    IsSuccess = signInResult.Succeeded,
                    Result = "Login failed, please check email or password",
                };

            if (!string.IsNullOrWhiteSpace(user.Id))
            {
                return new ResponseObject<string>
                {
                    IsSuccess = signInResult.Succeeded,
                    Result = _helper.CreateToken(user.Id, user.UserName!),
                    ErrorMessage = null,
                    Exception = null,
                };
            }

            return new ResponseObject<string>
            {
                Result = null,
                ErrorMessage = "Login failed, please check email or password",
                IsSuccess = signInResult.Succeeded,
                Exception = null,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "login going wrong");
            return new ResponseObject<string>
            {
                Exception = e,
                ErrorMessage = e.Message,
                IsSuccess = false,
                Result = null,
            };
        }

    }

    #endregion
}